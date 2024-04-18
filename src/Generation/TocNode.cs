// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a node in the TOC.
/// </summary>
public class TocNode
{
    /// <summary>
    /// Gets or sets the name of the node.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the path to an overview topic for this node.
    /// </summary>
    public string? Overview { get; set; }

    /// <summary>
    /// Gets or sets the resources within this node.
    /// </summary>
    public List<string>? Resources { get; set; }

    /// <summary>
    /// Gets or sets the complex types within this node.
    /// </summary>
    public List<string>? ComplexTypes { get; set; }

    /// <summary>
    /// Gets or sets the child nodes of this node.
    /// </summary>
    public List<TocNode>? ChildNodes { get; set; }

    /// <summary>
    /// Gets or sets the additional links of this node.
    /// </summary>
    public List<TocLink>? AdditionalLinks { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the section's nodes should be alphabetically sorted.
    /// </summary>
    public bool ShouldSort { get; set; }
}
