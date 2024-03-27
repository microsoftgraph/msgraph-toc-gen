// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Xml.Linq;

namespace GenerateTOC.Extensions;

/// <summary>
/// Contains extensions to classes from the System.Xml.Linq library.
/// </summary>
public static class XmlExtensions
{
    /// <summary>
    /// Gets all descendant elements in the <see cref="XDocument"/> that have the specified name.
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/> to get elements from.</param>
    /// <param name="name">The name of the elements to get.</param>
    /// <returns>A list of <see cref="XElement"/> objects with the specified name.</returns>
    public static IEnumerable<XElement> GetDescendants(this XDocument document, string name)
    {
        return document.Descendants().Where(e => e.Name.LocalName == name);
    }

    /// <summary>
    /// Gets all child elements in the <see cref="XElement"/> that have the specified name.
    /// </summary>
    /// <param name="element">The <see cref="XElement"/> to get elements from.</param>
    /// <param name="name">The name of the elements to get.</param>
    /// <returns>A list of <see cref="XElement"/> objects with the specified name.</returns>
    public static IEnumerable<XElement> GetElements(this XElement element, string name)
    {
        return element.Elements().Where(e => e.Name.LocalName == name);
    }

    /// <summary>
    /// Gets an attribute from an <see cref="XElement"/> by name.
    /// </summary>
    /// <param name="element">The <see cref="XElement"/> to get the attribute from.</param>
    /// <param name="name">The name of the attribute.</param>
    /// <returns>The attribute value, if present.</returns>
    public static string? GetAttribute(this XElement element, string name)
    {
        return element.Attributes().FirstOrDefault(a => a.Name.LocalName == name) is XAttribute attribute ?
            attribute.Value : null;
    }

    /// <summary>
    /// Checks if an entity represented by an <see cref="XElement"/> instance has AGS markup indicating it belongs in the workload.
    /// </summary>
    /// <param name="element">The <see cref="XElement"/> to check.</param>
    /// <returns>True if the entity belongs in the workload.</returns>
    public static bool BelongsInWorkload(this XElement element)
    {
        var isOwner = element.GetAttribute("IsOwner");
        var isShared = element.GetAttribute("IsSharedEntity");
        var isOwnerlessSingleton = element.GetAttribute("IsOwnerlessSingleton");

        return (!string.IsNullOrEmpty(isOwner) && isOwner.IsEqualIgnoringCase("true")) ||
                (!string.IsNullOrEmpty(isShared) && isShared.IsEqualIgnoringCase("true")) ||
                (!string.IsNullOrEmpty(isOwnerlessSingleton) && isOwnerlessSingleton.IsEqualIgnoringCase("true"));
    }
}
