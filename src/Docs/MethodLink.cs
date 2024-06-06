// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.Extensions;

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

    /// <summary>
    /// Checks if the file path is valid.
    /// </summary>
    /// <param name="resourceDocumentFilePath">The path to the resource document that contains this link.</param>
    /// <returns>A value indicating if the link is valid.</returns>
    public bool IsValid(string resourceDocumentFilePath)
    {
        // Is this a file path?

        var filePath = FilePath.NormalizeFilePath().TrimAnchor();
        var fileExtension = Path.GetExtension(filePath);
        if (string.IsNullOrEmpty(fileExtension))
        {
            // Assume this is link outside of the docset
            // (relative URL) and treat it as valid.
            return true;
        }

        // File extension should be either .md or .yml
        if (fileExtension.IsEqualIgnoringCase(".md") || fileExtension.IsEqualIgnoringCase(".yml"))
        {
            return File.Exists(filePath.FullPathRelativeToFile(resourceDocumentFilePath));
        }

        return false;
    }
}
