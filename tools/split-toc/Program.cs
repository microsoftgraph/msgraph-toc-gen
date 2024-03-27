// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.CommandLine;
using System.Text.RegularExpressions;
using GenerateTOC.Generation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var tocOption = new Option<FileInfo?>(
    aliases: ["--toc", "-t"],
    description: "The path to the toc.yml to split",
    parseArgument: result =>
    {
        var filePath = result.Tokens.Single().Value;
        if (!File.Exists(filePath))
        {
            result.ErrorMessage = $"{filePath} does not exist";
            return null;
        }
        return new FileInfo(filePath);
    })
    { IsRequired = true };

var outDirectoryOption = new Option<DirectoryInfo>(
    aliases: ["--out", "-o"],
    description: "The path to the directory where the split files should be saved")
    { IsRequired = true };

var updateOriginalOption = new Option<bool>(
    aliases: ["--update", "-u"],
    description: "Update the original TOC file with relative links to split files"
);

var rootCommand = new RootCommand();
rootCommand.AddOption(tocOption);
rootCommand.AddOption(outDirectoryOption);
rootCommand.AddOption(updateOriginalOption);

rootCommand.SetHandler(async (context) =>
{
    var tocFile = context.ParseResult.GetValueForOption(tocOption) ??
        throw new ArgumentException("The --toc option is required.");
    var outDirectory = context.ParseResult.GetValueForOption(outDirectoryOption) ??
        throw new ArgumentException("The --out option is required.");
    var updateOriginal = context.ParseResult.GetValueForOption(updateOriginalOption);

    var tocYaml = await File.ReadAllTextAsync(tocFile.FullName);
    var tocDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    var originalToc = tocDeserializer.Deserialize<YamlToc>(tocYaml);

    // Find the API reference node
    var apiReferenceRegex = new Regex("^API\\s.*[Rr]eference$");
    var referenceNodes = originalToc.Items.Where(i => apiReferenceRegex.IsMatch(i.Name ?? string.Empty));
    if (referenceNodes == null || !referenceNodes.Any())
    {
        Console.WriteLine("No API reference node detected in provided TOC file");
        context.ExitCode = 1;
        return;
    }

    if (referenceNodes.Count() > 1)
    {
        Console.WriteLine("More than one API reference node detected in provided TOC file");
        context.ExitCode = 1;
        return;
    }

    var referenceNode = referenceNodes.First();
    if (referenceNode != null && referenceNode.Items != null)
    {
        // Get child nodes that have children
        var workloadNodes = referenceNode.Items.Where(i => i.Items != null);
        if (workloadNodes == null || !workloadNodes.Any())
        {
            Console.WriteLine("No child nodes found under API reference node");
            context.ExitCode = 1;
            return;
        }

        // Empty out directory
        foreach (var file in outDirectory.EnumerateFiles())
        {
            file.Delete();
        }

        foreach (var directory in outDirectory.EnumerateDirectories())
        {
            directory.Delete(true);
        }

        var tocSerializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        foreach (var workloadNode in workloadNodes)
        {
            // Create a new TOC
            var workloadToc = new YamlToc()
            {
                Items = workloadNode.Items ?? throw new Exception("Items cannot be null"),
            };

            var directoryName = workloadNode.Name?.ToLower().Replace(' ', '-') ??
                throw new Exception("TOC node must have a name");

            // Create the subdirectory for this TOC
            var tocDirectory = outDirectory.CreateSubdirectory(directoryName) ??
                throw new Exception("Could not create subdirectory");
            var workloadTocFile = Path.Combine(tocDirectory.FullName, "toc.yml");

            var workloadTocYaml = tocSerializer.Serialize(workloadToc);
            await File.WriteAllTextAsync(workloadTocFile, workloadTocYaml);
            Console.WriteLine($"Created {workloadTocFile}");

            if (updateOriginal)
            {
                workloadNode.Items = null;

                var relativePath = Path.GetRelativePath(tocFile.FullName, workloadTocFile);
                workloadNode.Href = relativePath.Replace("\\", "/");
            }
        }

        if (updateOriginal)
        {
            var updatedTocYaml = tocSerializer.Serialize(originalToc);
            await File.WriteAllTextAsync(tocFile.FullName, updatedTocYaml);
            Console.WriteLine($"Updated {tocFile.FullName}");
        }
    }
});

Environment.Exit(await rootCommand.InvokeAsync(args));
