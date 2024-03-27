// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents the mapping JSON file structure.
/// </summary>
public class TocMapping
{
    /// <summary>
    /// Gets or sets the list of TOC nodes from the JSON file.
    /// </summary>
    public List<TocNode>? TocNodes { get; set; }

    /// <summary>
    /// Gets or sets the list of resource overviews.
    /// </summary>
    public List<ResourceOverview>? ResourceOverviews { get; set; }
}
