// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.CommandLine;
using GenerateTOC.Generation;
using GenerateTOC.Logging;

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

var csdlOption = new Option<string>(["--csdl", "-c"])
{
    Description = "The path to a folder containing the workload CSDL files",
    IsRequired = true,
};

var mappingOption = new Option<string>(["--mapping", "-m"])
{
    Description = "The path to the workload mapping JSON file",
    IsRequired = true,
};

var termsOverrideOption = new Option<string>(["--terms-override"])
{
    Description = "The path to the terms override JSON file",
    IsRequired = false,
};

var tocOption = new Option<string>(["--toc", "-t"])
{
    Description = "The path where the generated TOC file should be saved",
    IsRequired = true,
};

var staticTocOption = new Option<string>(["--static-toc", "-s"])
{
    Description = "The path to the static TOC file",
    IsRequired = false,
};

var logOption = new Option<string>(["--log-file", "-l"])
{
    Description = "The path to save output logs",
    IsRequired = false,
};

var versionOption = new Option<string>(["--api-version"])
{
    Description = "The API version to generate TOC for",
    IsRequired = false,
};

var rootCommand = new RootCommand();
rootCommand.AddOption(apiDocsOption);
rootCommand.AddOption(resourceDocsOption);
rootCommand.AddOption(csdlOption);
rootCommand.AddOption(mappingOption);
rootCommand.AddOption(termsOverrideOption);
rootCommand.AddOption(tocOption);
rootCommand.AddOption(staticTocOption);
rootCommand.AddOption(logOption);
rootCommand.AddOption(versionOption);

rootCommand.SetHandler(async (context) =>
{
    var generatorOptions = new GeneratorOptions
    {
        ApiDocsFolder = context.ParseResult.GetValueForOption(apiDocsOption) ??
            throw new ArgumentException("The --api-docs option is required."),
        ResourceDocsFolder = context.ParseResult.GetValueForOption(resourceDocsOption) ??
            throw new ArgumentException("The --resource-docs option is required."),
        CsdlFolder = context.ParseResult.GetValueForOption(csdlOption) ??
            throw new ArgumentException("The --csdl option is required."),
        MappingFile = context.ParseResult.GetValueForOption(mappingOption) ??
            throw new ArgumentException("The --mapping option is required."),
        TermsOverrideFile = context.ParseResult.GetValueForOption(termsOverrideOption),
        TocFile = context.ParseResult.GetValueForOption(tocOption) ??
            throw new ArgumentException("The --toc option is required."),
        StaticTocFile = context.ParseResult.GetValueForOption(staticTocOption),
        Version = context.ParseResult.GetValueForOption(versionOption),
    };
    generatorOptions.NormalizeFilePaths();

    var logFile = context.ParseResult.GetValueForOption(logOption);

    var logger = OutputLoggerFactory.GetLogger<Program>(logFile);

    var generator = new TableOfContentsGenerator(generatorOptions, logger);

    await generator.GenerateTocAsync();
});

Environment.Exit(await rootCommand.InvokeAsync(args));
