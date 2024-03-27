// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a TOC in toc.yml.
/// </summary>
public class YamlToc
{
    /// <summary>
    /// Gets or sets the list of nodes in a TOC.
    /// </summary>
    public List<YamlTocNode> Items { get; set; } = [];
}
