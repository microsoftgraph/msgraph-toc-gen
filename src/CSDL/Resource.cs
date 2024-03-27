// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Xml.Linq;
using GenerateTOC.Extensions;

namespace GenerateTOC.CSDL;

/// <summary>
/// Represents a resource in Microsoft Graph.
/// </summary>
/// <param name="name">The name of the resource.</param>
/// <param name="graphNamespace">The Microsoft Graph namespace the resource belongs to.</param>
/// <param name="workload">The workload the resource belongs to.</param>
/// <param name="baseType">The base type of the resource.</param>
/// <param name="isHidden">A value indicating whether the resource is hidden in the CSDL.</param>
public class Resource(string name, string graphNamespace, string workload, string? baseType, bool isHidden) : IComparable<Resource>
{
    /// <summary>
    /// Gets the name of the resource.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Gets the workload the resource belongs to.
    /// </summary>
    public string Workload { get; private set; } = workload;

    /// <summary>
    /// Gets the Microsoft Graph namespace the resource belongs to.
    /// </summary>
    public string GraphNamespace { get; private set; } = graphNamespace;

    /// <summary>
    /// Gets the base type of the resource.
    /// </summary>
    public string? BaseType { get; private set; } = baseType;

    /// <summary>
    /// Gets a value indicating whether the resource is hidden in the CSDL.
    /// </summary>
    public bool IsHidden { get; private set; } = isHidden;

    /// <summary>
    /// Creates an instance of <see cref="Resource"/> from an <see cref="XElement"/>.
    /// </summary>
    /// <param name="element">The XML element to create from.</param>
    /// <param name="graphNamespace">The Microsoft Graph namespace the resource belongs to.</param>
    /// <param name="workload">The workload the resource belongs to.</param>
    /// <returns>A new instance of <see cref="Resource"/>.</returns>
    public static Resource? CreateFromXElement(XElement? element, string graphNamespace, string workload)
    {
        // Check for IsHidden="true"
        var isHidden = element?.GetAttribute("IsHidden") is string isHiddenValue &&
            string.Equals(isHiddenValue, "true");

        if (element?.GetAttribute("Name") is string name)
        {
            return new Resource(
                name,
                graphNamespace,
                workload,
                element.GetAttribute("BaseType"),
                isHidden);
        }

        return null;
    }

    /// <inheritdoc/>
    public int CompareTo(Resource? other)
    {
        return string.CompareOrdinal(Name, other?.Name);
    }
}
