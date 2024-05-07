// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.CommandLine;
using System.Text.Json;
using System.Xml.Linq;
using GenerateTOC.CSDL;
using GenerateTOC.Extensions;

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
};

var csdlOption = new Option<FileInfo?>(
    aliases: ["--csdl", "-c"],
    description: "The path to the CSDL file to generate a mapping from",
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

var rootCommand = new RootCommand();
rootCommand.AddOption(csdlOption);

rootCommand.SetHandler(async (context) =>
{
    var csdlFile = context.ParseResult.GetValueForOption(csdlOption) ??
        throw new ArgumentException("The --csdl option is required.");

    var csdl = XDocument.Load(csdlFile.FullName);

    var namespaceElements = csdl.GetDescendants("Schema");

    var resourceNames = new List<string>();
    var complexTypeNames = new List<string>();
    foreach (var namespaceElement in namespaceElements)
    {
        var resources = WorkloadCollection.GetResourcesFromNamespace(namespaceElement, string.Empty);

        foreach (var resource in resources ?? [])
        {
            var name = resource.GraphNamespace.IsEqualIgnoringCase("microsoft.graph") ?
                    resource.Name : $"{resource.GraphNamespace}.{resource.Name}";

            if (resource.IsComplexType)
            {
                complexTypeNames.Add(name);
            }
            else
            {
                resourceNames.Add(name);
            }
        }
    }

    if (resourceNames.Count > 0 || complexTypeNames.Count > 0)
    {
        var jsonOut = new { resources = resourceNames, complexTypes = complexTypeNames };
        var jsonString = JsonSerializer.Serialize(jsonOut, jsonOptions);
        await File.WriteAllTextAsync("mapping.json", jsonString);
        Console.WriteLine("Resources and complex types written to mapping.json");
    }
});

Environment.Exit(await rootCommand.InvokeAsync(args));
