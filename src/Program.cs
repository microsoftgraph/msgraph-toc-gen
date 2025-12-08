// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.CommandLine;
using GenerateTOC.Generation;
using GenerateTOC.Logging;

var apiDocsOption = new Option<string>("--api-docs", "-a")
{
    Description = "The path to a folder containing the API docs",
    Required = true,
};

var resourceDocsOption = new Option<string>("--resource-docs", "-r")
{
    Description = "The path to a folder containing the resource docs",
    Required = true,
};

var mappingOption = new Option<string>("--mapping", "-m")
{
    Description = "The path to the workload mapping JSON file",
    Required = true,
};

var termsOverrideOption = new Option<string>("--terms-override")
{
    Description = "The path to the terms override JSON file",
    Required = false,
};

var tocOption = new Option<string>("--toc", "-t")
{
    Description = "The path where the generated TOC file should be saved",
    Required = true,
};

var staticTocOption = new Option<string>("--static-toc", "-s")
{
    Description = "The path to the static TOC file",
    Required = false,
};

var logOption = new Option<string>("--log-file", "-l")
{
    Description = "The path to save output logs",
    Required = false,
};

var versionOption = new Option<string>("--api-version")
{
    Description = "The API version to generate TOC for",
    Required = false,
};

var rootCommand = new RootCommand()
{
    apiDocsOption,
    resourceDocsOption,
    mappingOption,
    termsOverrideOption,
    tocOption,
    staticTocOption,
    logOption,
    versionOption,
};

rootCommand.SetAction(async (result) =>
{
    var generatorOptions = new GeneratorOptions
    {
        ApiDocsFolder = result.GetValue(apiDocsOption) ??
            throw new ArgumentException("The --api-docs option is required."),
        ResourceDocsFolder = result.GetValue(resourceDocsOption) ??
            throw new ArgumentException("The --resource-docs option is required."),
        MappingFile = result.GetValue(mappingOption) ??
            throw new ArgumentException("The --mapping option is required."),
        TermsOverrideFile = result.GetValue(termsOverrideOption),
        TocFile = result.GetValue(tocOption) ??
            throw new ArgumentException("The --toc option is required."),
        StaticTocFile = result.GetValue(staticTocOption),
        Version = result.GetValue(versionOption),
    };
    generatorOptions.NormalizeFilePaths();

    var logFile = result.GetValue(logOption);
    var logger = OutputLoggerFactory.GetLogger<Program>(logFile);

    var generator = new TableOfContentsGenerator(generatorOptions, logger);

    await generator.GenerateTocAsync();
});

Environment.Exit(await rootCommand.Parse(args).InvokeAsync());
