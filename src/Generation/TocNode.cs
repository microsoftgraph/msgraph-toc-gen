// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Text.Json;

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a node in the TOC.
/// </summary>
public class TocNode
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Gets or sets the name of the node.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the path to an overview topic for this node.
    /// </summary>
    public string? Overview { get; set; }

    /// <summary>
    /// Gets or sets the keywords for this node.
    /// </summary>
    public List<string>? Keywords { get; set; }

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

    /// <summary>
    /// Gets or sets a path to a JSON file containing this node's definition.
    /// </summary>
    public string? File { get; set; }

    /// <summary>
    /// Loads properties from the File property if set.
    /// </summary>
    /// <param name="parentMappingFilePath">The path to the parent mapping file.</param>
    /// <returns>The loaded <see cref="TocNode"/>.</returns>
    public TocNode? LoadIfNeeded(string parentMappingFilePath)
    {
        if (string.IsNullOrEmpty(File))
        {
            return this;
        }

        var mappingDirectory = Path.GetDirectoryName(parentMappingFilePath);
        if (!string.IsNullOrEmpty(mappingDirectory))
        {
            var fileContents = System.IO.File.ReadAllText(Path.Combine(mappingDirectory, File));
            return JsonSerializer.Deserialize<TocNode>(fileContents, JsonOptions);
        }

        return null;
    }
}
