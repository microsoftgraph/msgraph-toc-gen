// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents an overview for a resource.
/// </summary>
public class ResourceOverview
{
    /// <summary>
    /// Gets or sets the name of the resource.
    /// </summary>
    public string? Resource { get; set; }

    /// <summary>
    /// Gets or sets the path to the overview.
    /// </summary>
    public string? Overview { get; set; }
}
