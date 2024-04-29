// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.Docs.Extensions;
using GenerateTOC.Extensions;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GenerateTOC.Docs;

/// <summary>
/// Represents a Markdown file that documents a Microsoft Graph API.
/// </summary>
public class ResourceDocument
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UsePipeTables()
        .UseYamlFrontMatter()
        .Build();

    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceDocument"/> class.
    /// </summary>
    /// <param name="filePath">The path to the Markdown file to load from.</param>
    internal ResourceDocument(string filePath)
    {
        FilePath = filePath;
        Methods = [];
    }

    /// <summary>
    /// Gets the list of API operations in this document.
    /// </summary>
    public List<MethodLink> Methods { get; private set; }

    /// <summary>
    /// Gets the path to the Markdown file for this document.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// Gets the name of the resource documented in this document.
    /// </summary>
    public string? ResourceName { get; private set; }

    /// <summary>
    /// Gets the namespace declared in the document.
    /// </summary>
    public string? GraphNameSpace { get; private set; }

    /// <summary>
    /// Gets the TOC title override declared in the document's toc.title YAML value.
    /// </summary>
    public string? TocTitleOverride { get; private set; }

    /// <summary>
    /// Gets the keywords declared in the document's toc.keywords YAML value.
    /// </summary>
    public List<string>? Keywords { get; private set; }

    private MarkdownDocument? MarkdownDocument { get; set; }

    /// <summary>
    /// Creates an instance of the <see cref="ResourceDocument"/> class from a Markdown file.
    /// </summary>
    /// <param name="filePath">The path to the Markdown file to create from.</param>
    /// <param name="logger">Instance of <see cref="ILogger"/> for output.</param>
    /// <returns>A task representing the asynchronous create operation. The result of the task contains the created <see cref="ResourceDocument"/>.</returns>
    public static async Task<ResourceDocument?> CreateFromMarkdownFile(string filePath, ILogger logger)
    {
        try
        {
            var doc = new ResourceDocument(filePath);
            await doc.LoadMarkdown();
            return doc;
        }
        catch (DocTypeException dex)
        {
            logger.LogDebug(
                GenerationEventId.WrongDocType,
                "Error parsing {file}: {msg}",
                filePath,
                dex.Message);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                GenerationEventId.ResourceDocParseError,
                "Error parsing {file}: {msg}",
                filePath,
                ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Loads Markdown content into this instance of <see cref="ResourceDocument"/>.
    /// </summary>
    /// <param name="markdownContent">The Markdown content.</param>
    /// <exception cref="Exception">Throws if Markdown content does not conform to a resource document's structure.</exception>
    internal void LoadMarkdown(string markdownContent)
    {
        var docType = markdownContent.ExtractDocType();
        if (string.IsNullOrEmpty(docType) || !string.Equals(docType, "resourcePageType", StringComparison.OrdinalIgnoreCase))
        {
            throw new DocTypeException($"File is not a resource document - doc_type: {docType ?? "NONE"}");
        }

        ResourceName = markdownContent.ExtractResourceName() ??
            throw new Exception($"Could not determine resource name from file");
        GraphNameSpace = markdownContent.ExtractNamespace();

        MarkdownDocument = Markdown.Parse(markdownContent, Pipeline);

        var yamlBlock = MarkdownDocument.Descendants<YamlFrontMatterBlock>().FirstOrDefault();
        if (yamlBlock != null)
        {
            var yaml = markdownContent.Substring(yamlBlock.Span.Start, yamlBlock.Span.Length).TrimEnd('-');
            var docYamlBlock = YamlDeserializer.Deserialize<DocYamlBlock>(yaml);
            TocTitleOverride = docYamlBlock.TocTitle;
            Keywords = docYamlBlock.Keywords;
        }

        // Find the "Methods" block
        var insideMethodsSection = false;
        var requestHeadingLevel = 0;
        string? subHeading = null;
        foreach (var block in MarkdownDocument.ToList())
        {
            if (!insideMethodsSection)
            {
                if (block is HeadingBlock headingBlock && headingBlock.TextEquals("Methods"))
                {
                    insideMethodsSection = true;
                    requestHeadingLevel = headingBlock.Level;
                }

                continue;
            }

            // Parse the Markdown table to build a list of titles + relative link
            // relative link probably needs to be modified to be correct for the TOC
            // Just cut everything before "api"
            if (block is Table table)
            {
                // Make sure it is actually a Method table
                var headings = table.GetColumnHeadings();
                if (string.Equals(headings[0], "Method", StringComparison.OrdinalIgnoreCase))
                {
                    var rows = table.ToList();
                    rows.RemoveAt(0);
                    foreach (var row in rows)
                    {
                        var methodCell = row.Descendants<TableCell>().First();
                        var methodLink = methodCell.ParseMethodCell(subHeading);
                        if (methodLink != null)
                        {
                            Methods.Add(methodLink);
                        }
                        else
                        {
                            subHeading = methodCell.TextValue() ?? subHeading;
                        }
                    }
                }
            }

            // Stop looking at next H2
            if (block is HeadingBlock nextHeading)
            {
                if (nextHeading.Level <= requestHeadingLevel)
                {
                    break;
                }
                else if (nextHeading.Level == requestHeadingLevel + 1)
                {
                    subHeading = nextHeading.TextValue();
                }
            }
        }
    }

    private async Task LoadMarkdown()
    {
        using var markdownFile = File.OpenRead(FilePath);
        using var streamReader = new StreamReader(markdownFile);

        var markdownContent = await streamReader.ReadToEndAsync();
        LoadMarkdown(markdownContent);
    }
}
