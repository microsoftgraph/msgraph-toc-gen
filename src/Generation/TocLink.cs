// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a TOC link.
/// </summary>
public class TocLink
{
    /// <summary>
    /// Gets or sets the name of the link.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the URL of the link.
    /// </summary>
    public string? Href { get; set; }
}
