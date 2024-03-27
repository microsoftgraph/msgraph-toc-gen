// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a workload within a TOC node.
/// </summary>
public class TocWorkload
{
    /// <summary>
    /// Gets or sets the workload identifier.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the list of excluded resources.
    /// </summary>
    public List<string>? ExcludedResources { get; set; }

    /// <summary>
    /// Gets or sets the list of included resources.
    /// </summary>
    public List<string>? IncludedResources { get; set; }
}
