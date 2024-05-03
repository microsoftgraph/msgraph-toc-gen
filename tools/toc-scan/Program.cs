// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.CommandLine;
using System.Net.Mime;
using System.Text.RegularExpressions;
using TOCScan;

var apiDocsOption = new Option<string>(["--api-docs", "-a"])
{
    Description = "The path to a folder containing the API docs",
    IsRequired = true,
};

var resourceDocsOption = new Option<string>(["--resource-docs", "-r"])
{
    Description = "The path to a folder containing the resource docs",
    IsRequired = true,
};

var tocOption = new Option<string>(["--toc", "-t"])
{
    Description = "The path to a folder containing the TOC files",
    IsRequired = true,
};

var rootCommand = new RootCommand();
rootCommand.AddOption(apiDocsOption);
rootCommand.AddOption(resourceDocsOption);
rootCommand.AddOption(tocOption);

rootCommand.SetHandler(async (context) =>
{
    var apiDocsFolder = context.ParseResult.GetValueForOption(apiDocsOption) ??
        throw new ArgumentException("The --api-docs option is required.");
    var resourceDocsFolder = context.ParseResult.GetValueForOption(resourceDocsOption) ??
        throw new ArgumentException("The --resource-docs option is required.");
    var tocFolder = context.ParseResult.GetValueForOption(tocOption) ??
        throw new ArgumentException("The --toc option is required.");

    // Get list of API docs
    var apiDocFiles = Directory.GetFiles(apiDocsFolder).ToList();

    // Get list of resource docs
    var resourceFiles = Directory.GetFiles(resourceDocsFolder).ToList();

    // Get list of TOC files
    var tocFiles = Directory.GetFiles(tocFolder, "toc.yml", SearchOption.AllDirectories);

    // Parse TOC files and generate a list of resource and API docs included
    // in the TOC files
    var resourcesInTOC = new List<string>();
    var apisInTOC = new List<string>();
    foreach (var tocFile in tocFiles)
    {
        var tocContents = await File.ReadAllTextAsync(tocFile);

        var resourceMatches = TOCEntries.ResourceRegex().Matches(tocContents);
        foreach (var match in resourceMatches.Cast<Match>())
        {
            resourcesInTOC.Add(match.Groups["file"].Value);
        }

        var apiMatches = TOCEntries.ApiRegex().Matches(tocContents);
        foreach (var match in apiMatches.Cast<Match>())
        {
            apisInTOC.Add(match.Groups["file"].Value);
        }
    }

    // Eliminate duplicates
    resourcesInTOC = resourcesInTOC.Distinct().ToList();
    apisInTOC = apisInTOC.Distinct().ToList();

    // Find docs not in TOC
    var resourcesNotInTOC = resourceFiles.Select(f => Path.GetFileName(f)).Where(f => !resourcesInTOC.Contains(f)).ToList();
    var apisNotInTOC = apiDocFiles.Select(f => Path.GetFileName(f)).Where(f => !apisInTOC.Contains(f)).ToList();

    Console.WriteLine($"{resourcesNotInTOC.Count} of {resourceFiles.Count} resource documents not found in any TOC.");
    Console.WriteLine($"{apisNotInTOC.Count} of {apiDocFiles.Count} api documents not found in any TOC.");

    await File.WriteAllLinesAsync("files.txt", resourcesNotInTOC);
    await File.AppendAllLinesAsync("files.txt", apisNotInTOC);
});

Environment.Exit(await rootCommand.InvokeAsync(args));
