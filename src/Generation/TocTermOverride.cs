// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Represents a term override to control capitalization.
/// </summary>
public class TocTermOverride
{
    /// <summary>
    /// Gets or sets the term to override. Case-insensitive.
    /// </summary>
    public string? Term { get; set; }

    /// <summary>
    /// Gets or sets the correct capitalization of the term.
    /// </summary>
    public string? Override { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the override is case sensitive.
    /// </summary>
    public bool CaseSensitive { get; set; }
}
