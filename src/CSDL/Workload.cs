// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Xml.Linq;

namespace GenerateTOC.CSDL;

/// <summary>
/// Represents a Microsoft Graph workload.
/// </summary>
public class Workload
{
    /// <summary>
    /// Gets or sets the workload identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the <see cref="XDocument"/> containing the CSDL file contents.
    /// </summary>
    public XDocument? Csdl { get; set; }

    /// <summary>
    /// Gets the resources found in the workload.
    /// </summary>
    public List<Resource> Resources { get; } = [];
}
