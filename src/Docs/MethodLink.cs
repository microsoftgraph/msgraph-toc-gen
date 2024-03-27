// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Docs;

/// <summary>
/// Represents a link to a method in the Microsoft Graph docs.
/// </summary>
/// <param name="title">The link title, used in the TOC.</param>
/// <param name="filePath">The file path for the link.</param>
/// <param name="heading">The subheading this method link was found under, if any.</param>
public class MethodLink(string title, string filePath, string? heading = null)
{
    /// <summary>
    /// Gets the link title, used in the TOC.
    /// </summary>
    public string Title => title;

    /// <summary>
    /// Gets the file path for the link.
    /// </summary>
    public string FilePath => filePath;

    /// <summary>
    /// Gets the subheading this method link was found under, if any.
    /// </summary>
    public string? Heading => heading;
}
