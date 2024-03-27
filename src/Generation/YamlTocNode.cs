// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using YamlDotNet.Serialization;

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a TOC node in toc.yml.
/// </summary>
public class YamlTocNode : IComparable<YamlTocNode>
{
    /// <summary>
    /// Gets or sets the name of the node.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the HREF of the node.
    /// </summary>
    public string? Href { get; set; }

    /// <summary>
    /// Gets or sets the display name of the node.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the node should be initially expanded.
    /// </summary>
    public bool Expanded { get; set; } = false;

    /// <summary>
    /// Gets or sets a list of child nodes.
    /// </summary>
    public List<YamlTocNode>? Items { get; set; }

    /// <summary>
    /// Gets or sets the resource name for this node.
    /// </summary>
    [YamlIgnore]
    public string? ResourceName { get; set; }

    /// <inheritdoc/>
    public int CompareTo(YamlTocNode? other)
    {
        return string.Compare(Name, other?.Name, StringComparison.OrdinalIgnoreCase);
    }
}
