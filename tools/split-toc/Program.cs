// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.CommandLine;
using System.Text.RegularExpressions;
using GenerateTOC.Generation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var tocOption = new Option<FileInfo?>("--toc", "-t")
{
    Description = "The path to the toc.yml to split",
    Required = true,
    CustomParser = result =>
    {
        var filePath = result.Tokens.Single().Value;
        if (!File.Exists(filePath))
        {
            result.AddError($"{filePath} does not exist");
            return null;
        }
        return new FileInfo(filePath);
    }
};

var outDirectoryOption = new Option<DirectoryInfo>("--out", "-o")
{
    Description = "The path to the directory where the split files should be saved",
    Required = true,
};

var updateOriginalOption = new Option<bool>("--update", "-u")
{
    Description = "Update the original TOC file with relative links to split files",
    Required = false,
};

var rootCommand = new RootCommand()
{
    tocOption,
    outDirectoryOption,
    updateOriginalOption,
};

rootCommand.SetAction(async (result) =>
{
    var tocFile = result.GetValue(tocOption) ??
        throw new ArgumentException("The --toc option is required.");
    var outDirectory = result.GetValue(outDirectoryOption) ??
        throw new ArgumentException("The --out option is required.");
    var updateOriginal = result.GetValue(updateOriginalOption);

    var tocYaml = await File.ReadAllTextAsync(tocFile.FullName);
    var tocDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    var originalToc = tocDeserializer.Deserialize<YamlToc>(tocYaml);

    // Find the API reference node
    var referenceNodes = originalToc.Items.Where(i => ApiReferenceRegex().IsMatch(i.Name ?? string.Empty));
    if (referenceNodes == null || !referenceNodes.Any())
    {
        Console.WriteLine("No API reference node detected in provided TOC file");
        return 1;
    }

    if (referenceNodes.Count() > 1)
    {
        Console.WriteLine("More than one API reference node detected in provided TOC file");
        return 1;
    }

    var referenceNode = referenceNodes.First();
    if (referenceNode != null && referenceNode.Items != null)
    {
        // Get child nodes that have children
        var workloadNodes = referenceNode.Items.Where(i => i.Items != null);
        if (workloadNodes == null || !workloadNodes.Any())
        {
            Console.WriteLine("No child nodes found under API reference node");
            return 1;
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
    return 0;
});

Environment.Exit(await rootCommand.Parse(args).InvokeAsync());

partial class Program
{
    [GeneratedRegex("^API\\s.*[Rr]eference$")]
    private static partial Regex ApiReferenceRegex();
}
