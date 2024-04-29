// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using YamlDotNet.Serialization;

namespace GenerateTOC.Docs;

/// <summary>
/// Represents the YAML front matter in a resource document.
/// </summary>
public class DocYamlBlock
{
    /// <summary>
    /// Gets or sets the value of the title field.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the value of the doc_type field.
    /// </summary>
    [YamlMember(Alias = "doc_type", ApplyNamingConventions = false)]
    public string? DocType { get; set; }

    /// <summary>
    /// Gets or sets the value of the toc.title field.
    /// </summary>
    [YamlMember(Alias = "toc.title", ApplyNamingConventions = false)]
    public string? TocTitle { get; set; }

    /// <summary>
    /// Gets or sets the value of the toc.keywords field.
    /// </summary>
    [YamlMember(Alias = "toc.keywords", ApplyNamingConventions = false)]
    public List<string>? Keywords { get; set; }
}
