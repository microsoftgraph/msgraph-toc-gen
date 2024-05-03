// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Text.Json;
using GenerateTOC.Docs;
using GenerateTOC.Extensions;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GenerateTOC.Generation;

/// <summary>
/// Generates a TOC for Microsoft Graph documentation.
/// </summary>
/// <param name="options">The options that control how generation is performed.</param>
/// <param name="logger">The logger for output.</param>
public class TableOfContentsGenerator(GeneratorOptions options, ILogger logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private GeneratorOptions Options => options;

    private ILogger Logger => logger;

    /// <summary>
    /// Generates the TOC.
    /// </summary>
    /// <returns>A task that represents the asynchronous generation operation.</returns>
    public async Task GenerateTocAsync()
    {
        Logger.LogInformation("Starting TOC generation with the following parameters:");
        Logger.LogInformation("  API docs folder: {folder}", Options.ApiDocsFolder);
        Logger.LogInformation("  Resource docs folder: {folder}", Options.ResourceDocsFolder);
        Logger.LogInformation("  Mapping file: {file}", Options.MappingFile);
        Logger.LogInformation("  Terms override file: {file}", Options.TermsOverrideFile ?? "NONE");
        Logger.LogInformation("  Output TOC: {file}", Options.TocFile);
        Logger.LogInformation("  Static TOC: {file}", Options.StaticTocFile ?? "NONE");
        Logger.LogInformation("  API Version: {version}", Options.Version ?? "v1.0");

        // Load the mapping file
        var tocMapping = await LoadMappingFileAsync();
        if (tocMapping == null || tocMapping.TocNodes == null)
        {
            return;
        }

        var termOverrides = await LoadTermOverridesFileAsync();
        StringExtensions.Initialize(termOverrides);

        // Build a list of resource docs and map to the known resources
        var docSet = await DocSet.CreateFromDirectory(Options.ResourceDocsFolder, Logger);
        Logger.LogInformation("Found {count} resource documents.", docSet.ResourceDocuments.Count);

        var outputToc = await InitializeYamlToc();

        var tocSerializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var subTocDirectory = Path.Combine(
            Path.GetDirectoryName(Options.TocFile) ?? ".",
            "toc");
        Directory.CreateDirectory(subTocDirectory);

        List<YamlTocNode> nodesToAdd = [];
        foreach (var node in tocMapping.TocNodes)
        {
            // Create a top-level node
            var yamlNode = BuildYamlTocNodeForTocNode(node, docSet, tocMapping.ResourceOverviews);

            // Create output TOC for this node
            if (yamlNode.Items != null && !string.IsNullOrEmpty(yamlNode.Name))
            {
                var outToc = new YamlToc()
                {
                    Items = yamlNode.Items,
                };

                var directoryName = yamlNode.Name.ToLower().Replace(' ', '-');
                var subDirectory = Path.Combine(subTocDirectory, directoryName);
                Directory.CreateDirectory(subDirectory);
                var subTocFileName = Path.Combine(subDirectory, "toc.yml");

                var subTocYaml = tocSerializer.Serialize(outToc);
                await File.WriteAllTextAsync(subTocFileName, subTocYaml);

                yamlNode.Items = null;
                yamlNode.Href = $"toc/{directoryName}/toc.yml";
            }

            nodesToAdd.Add(yamlNode);
        }

        if (outputToc.Items.Count > 0)
        {
            var lastChildNode = outputToc.Items.Last();
            lastChildNode.Items ??= [];
            lastChildNode.Items.AddRange(nodesToAdd);
        }
        else
        {
            outputToc.Items.AddRange(nodesToAdd);
        }

        // Write toc.yml
        var tocYaml = tocSerializer.Serialize(outputToc);
        await File.WriteAllTextAsync(Options.TocFile, tocYaml);
        Logger.LogInformation("TOC generation complete.");
    }

    private static List<YamlTocNode> MethodLinksToYamlTocNodes(IEnumerable<MethodLink> methods)
    {
        var tocNodes = new List<YamlTocNode>();

        foreach (var method in methods)
        {
            tocNodes.Add(new YamlTocNode
            {
                Name = method.Title,
                Href = method.FilePath.ToTocRelativePath(),
            });
        }

        return tocNodes;
    }

    private async Task<YamlToc> InitializeYamlToc()
    {
        if (string.IsNullOrEmpty(Options.StaticTocFile))
        {
            return new YamlToc();
        }

        // Load the static TOC
        var staticTocYaml = await File.ReadAllTextAsync(Options.StaticTocFile);
        var tocDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return tocDeserializer.Deserialize<YamlToc>(staticTocYaml);
    }

    private YamlTocNode BuildYamlTocNodeForTocNode(
        TocNode tocNode,
        DocSet docSet,
        List<ResourceOverview>? resourceOverviews)
    {
        tocNode = tocNode.LoadIfNeeded(Options.MappingFile) ?? tocNode;

        // Create a top-level node
        var yamlNode = new YamlTocNode
        {
            Name = tocNode.Name,
            Items = [],
        };

        if (!string.IsNullOrEmpty(tocNode.Overview))
        {
            yamlNode.Items.Add(new YamlTocNode
            {
                Name = "Overview",
                Href = tocNode.Overview.ToTocRelativePath(),
            });
        }

        if (tocNode.Keywords != null && tocNode.Keywords.Count > 0)
        {
            yamlNode.DisplayName = string.Join(", ", tocNode.Keywords);
        }

        foreach (var link in tocNode.AdditionalLinks ?? [])
        {
            yamlNode.Items.Add(new YamlTocNode
            {
                Name = link.Name,
                Href = link.Href,
            });
        }

        var unsortedNodes = new List<YamlTocNode>();
        var complexTypeNodes = new List<YamlTocNode>();

        foreach (var resource in tocNode.Resources ?? [])
        {
            var resourceNode = BuildYamlTocNodeForResource(resource, docSet, resourceOverviews);
            if (resourceNode != null)
            {
                unsortedNodes.Add(resourceNode);
            }
        }

        foreach (var complexType in tocNode.ComplexTypes ?? [])
        {
            var complexTypeNode = BuildYamlTocNodeForResource(complexType, docSet, resourceOverviews);
            if (complexTypeNode != null)
            {
                complexTypeNodes.Add(complexTypeNode);
            }
        }

        foreach (var childNode in tocNode.ChildNodes ?? [])
        {
            // Build the node for this child
            var childTocNode = BuildYamlTocNodeForTocNode(childNode, docSet, resourceOverviews);

            // Check for existing node
            var existingNode = unsortedNodes.SingleOrDefault(i => string.Compare(i.Name, childNode.Name) == 0);
            if (existingNode == null)
            {
                // Append those nodes
                // yamlNode.Items.Add(childTocNode);
                unsortedNodes.Add(childTocNode);
            }
            else
            {
                existingNode.Items ??= [];
                existingNode.Items.AddRange(childTocNode.Items ?? []);
            }
        }

        if (tocNode.ShouldSort)
        {
            unsortedNodes.Sort();
        }

        yamlNode.Items.AddRange(unsortedNodes);

        if (complexTypeNodes.Count > 0)
        {
            // Always sort complex types alphabetically
            complexTypeNodes.Sort();
            yamlNode.Items.Add(new YamlTocNode
            {
                Name = "Complex types",
                Items = complexTypeNodes,
            });
        }

        return yamlNode;
    }

    private YamlTocNode? BuildYamlTocNodeForResource(string resourceName, DocSet docSet, List<ResourceOverview>? resourceOverviews)
    {
        var resource = resourceName.ToResource();

        try
        {
            ResourceDocument? resourceDoc = null;

            // Find the resource's document
            var potentialMatches = docSet.ResourceDocuments.Where(rd =>
                (rd.ResourceName?.IsEqualIgnoringCase(resource.Name) ?? false) &&
                (rd.GraphNameSpace?.IsEqualIgnoringCase(resource.GraphNamespace) ?? false)).ToList();

            if (potentialMatches.Count == 1)
            {
                resourceDoc = potentialMatches[0];
            }
            else if (potentialMatches.Count > 0)
            {
                var fileNames = string.Join(",", potentialMatches.Select(rd => Path.GetFileName(rd.FilePath)).ToArray());
                Logger.LogWarning(
                    GenerationEventId.MultipleDocsFound,
                    "Found {count} documents for {namespace}.{resource}: {files}",
                    potentialMatches.Count,
                    resource.GraphNamespace,
                    resource.Name,
                    fileNames);

                // Fall back on trying to find a file with the resource name
                resourceDoc = potentialMatches.SingleOrDefault(
                    rd => Path.GetFileNameWithoutExtension(rd.FilePath).IsEqualIgnoringCase(resource.Name));

                // If that didn't work, it may be an Intune "shared" resource.
                resourceDoc ??= potentialMatches.SingleOrDefault(
                        rd => Path.GetFileName(rd.FilePath).StartsWith("intune-shared"));
            }

            _ = resourceDoc ?? throw new Exception("No matches found");

            var resourceTocNodeName = resourceDoc.TocTitleOverride ?? resource.Name.SplitCamelCaseToSentenceCase();
            var overview = resourceOverviews?.SingleOrDefault(o => o.Resource != null && o.Resource.IsEqualIgnoringCase(resource.Name));

            // If the resource has not methods and no overview,
            // it doesn't need to be an expandable node, just return
            // a flat node with a link to the resource.
            if (resourceDoc.Methods.Count == 0 && overview == null)
            {
                return new YamlTocNode
                {
                    Name = resourceTocNodeName,
                    ResourceName = resource.Name,
                    Href = resourceDoc.FilePath.ToTocRelativePath(),
                    DisplayName = resourceDoc.Keywords != null ? string.Join(", ", resourceDoc.Keywords) : null,
                };
            }

            var yamlNode = new YamlTocNode
            {
                Name = resourceTocNodeName,
                ResourceName = resource.Name,
                DisplayName = resourceDoc.Keywords != null ? string.Join(", ", resourceDoc.Keywords) : null,
                Items =
                [
                    new YamlTocNode
                    {
                        Name = resourceTocNodeName,
                        Href = resourceDoc.FilePath.ToTocRelativePath(),
                    },
                ],
            };

            if (overview != null)
            {
                yamlNode.Items.Insert(0, new YamlTocNode
                {
                    Name = "Overview",
                    Href = overview.Overview,
                });
            }

            var headings = resourceDoc.Methods.Select(m => m.Heading).Distinct();
            foreach (var heading in headings)
            {
                var methods = resourceDoc.Methods.Where(m => m.Heading == heading);
                var methodNodes = MethodLinksToYamlTocNodes(methods);

                if (string.IsNullOrEmpty(heading))
                {
                    yamlNode.Items.AddRange(methodNodes);
                }
                else
                {
                    yamlNode.Items.Add(new YamlTocNode
                    {
                        Name = heading,
                        Items = methodNodes,
                    });
                }
            }

            return yamlNode;
        }
        catch (Exception)
        {
            Logger.LogError(
                GenerationEventId.NoDocument,
                "Could not find resource document for {namespace}.{resource}",
                resource.GraphNamespace,
                resource.Name);
            return null;
        }
    }

    private async Task<TocMapping?> LoadMappingFileAsync()
    {
        if (string.IsNullOrEmpty(Options.MappingFile))
        {
            Logger.LogError(GenerationEventId.InvalidMappingFile, "No mapping file provided.");
            return null;
        }

        if (!File.Exists(Options.MappingFile))
        {
            Logger.LogError(
                GenerationEventId.InvalidMappingFile,
                "Invalid mapping file {file}. File was not found.",
                Options.MappingFile);
            return null;
        }

        try
        {
            var mappingJson = await File.ReadAllTextAsync(Options.MappingFile);
            return JsonSerializer.Deserialize<TocMapping>(mappingJson, JsonOptions);
        }
        catch (Exception ex)
        {
            Logger.LogError(
                GenerationEventId.InvalidMappingFile,
                "Error loading mapping file: {msg}",
                ex.Message);
            return null;
        }
    }

    private async Task<List<TocTermOverride>?> LoadTermOverridesFileAsync()
    {
        if (string.IsNullOrEmpty(Options.TermsOverrideFile))
        {
            return null;
        }

        if (!File.Exists(Options.TermsOverrideFile))
        {
            Logger.LogError(
                GenerationEventId.InvalidMappingFile,
                "Invalid terms override file {file}. File was not found.",
                Options.TermsOverrideFile);
            return null;
        }

        try
        {
            var termsOverrideJson = await File.ReadAllTextAsync(Options.TermsOverrideFile);
            return JsonSerializer.Deserialize<List<TocTermOverride>>(termsOverrideJson, JsonOptions);
        }
        catch (Exception ex)
        {
            Logger.LogError(
                GenerationEventId.InvalidMappingFile,
                "Error loading terms override file: {msg}",
                ex.Message);
            return null;
        }
    }
}
