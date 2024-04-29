// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents the options for the <see cref="TableOfContentsGenerator"/> class.
/// </summary>
public class GeneratorOptions
{
    /// <summary>
    /// Gets or sets the path to the folder that contains API documents.
    /// </summary>
    public string ApiDocsFolder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the folder that contains resource documents.
    /// </summary>
    public string ResourceDocsFolder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the mapping file.
    /// </summary>
    public string MappingFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the terms override file.
    /// </summary>
    public string? TermsOverrideFile { get; set; }

    /// <summary>
    /// Gets or sets the path to the generated TOC file.
    /// </summary>
    public string TocFile { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the static TOC file.
    /// </summary>
    public string? StaticTocFile { get; set; }

    /// <summary>
    /// Gets or sets the API version to generate TOC for.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// Normalizes file paths.
    /// </summary>
    public void NormalizeFilePaths()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        ApiDocsFolder = Path.Combine(currentDirectory, ApiDocsFolder);
        ResourceDocsFolder = Path.Combine(currentDirectory, ResourceDocsFolder);
        MappingFile = Path.Combine(currentDirectory, MappingFile);
        TermsOverrideFile = string.IsNullOrEmpty(TermsOverrideFile) ? null : Path.Combine(currentDirectory, TermsOverrideFile);
        TocFile = Path.Combine(currentDirectory, TocFile);
        StaticTocFile = string.IsNullOrEmpty(StaticTocFile) ? null :
            Path.Combine(currentDirectory, StaticTocFile);
    }
}
