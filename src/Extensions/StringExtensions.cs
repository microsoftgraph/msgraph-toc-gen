// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using GenerateTOC.CSDL;
using GenerateTOC.Generation;

namespace GenerateTOC.Extensions;

/// <summary>
/// Contains extensions for the <see cref="string"/> type.
/// </summary>
public static partial class StringExtensions
{
    private static List<TocTermOverride>? termOverrides;

    /// <summary>
    /// Initializes term overrides.
    /// </summary>
    /// <param name="overrides">The list of term overrides.</param>
    public static void Initialize(List<TocTermOverride>? overrides)
    {
        termOverrides = overrides;
    }

    /// <summary>
    /// Extracts the doc type from Markdown content.
    /// </summary>
    /// <param name="markdown">The Markdown content to extract from.</param>
    /// <returns>The doc type.</returns>
    public static string? ExtractDocType(this string markdown)
    {
        var matches = DocTypeFromYamlRegex().Matches(markdown);
        return matches.Count > 0 ? matches[0].Groups["docType"].Value : null;
    }

    /// <summary>
    /// Extracts the resource name from Markdown content.
    /// </summary>
    /// <param name="markdown">The Markdown content to extract from.</param>
    /// <returns>The resource name.</returns>
    public static string? ExtractResourceName(this string markdown)
    {
        var yamlMatches = TitleFromYamlRegex().Matches(markdown);
        if (yamlMatches.Count > 0)
        {
            return yamlMatches[0].Groups["resourceName"].Value;
        }

        var h1Matches = TitleFromH1Regex().Matches(markdown);
        if (h1Matches.Count > 0)
        {
            return h1Matches[0].Groups["resourceName"].Value;
        }

        return null;
    }

    /// <summary>
    /// Extracts the namespace from Markdown content.
    /// </summary>
    /// <param name="markdown">The Markdown content to extract from.</param>
    /// <returns>The namespace.</returns>
    public static string ExtractNamespace(this string markdown)
    {
        var matches = NamespaceLineRegex().Matches(markdown);
        if (matches.Count <= 0)
        {
            // Default to microsoft.graph
            return "microsoft.graph";
        }

        return matches[0].Groups["namespace"].Value;
    }

    /// <summary>
    /// Compares the string against a provided value, ignoring case.
    /// </summary>
    /// <param name="value">The string to act on.</param>
    /// <param name="compareTo">The string to compare to.</param>
    /// <returns>A value indicating whether the strings are equal, ignoring case.</returns>
    public static bool IsEqualIgnoringCase(this string value, string compareTo)
    {
        int compareValue = string.Compare(value, compareTo, StringComparison.InvariantCultureIgnoreCase);

        // Handle inconsistent formatting for function parameters in OpenAPI
        // Sometimes value placeholders are like '{value}',
        // Sometimes just like {value}
        if (compareValue != 0 && value.Contains('(') && value.Contains('{') && compareTo.Contains('(') && compareTo.Contains('{'))
        {
            var pattern = $"^{compareTo.Replace("'", "'?").Replace("(", "\\(").Replace(")", "\\)")}$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var isMatch = regex.IsMatch(value);
            return isMatch;
        }

        return compareValue == 0;
    }

    /// <summary>
    /// Converts a file path to the format expected by the TOC.
    /// </summary>
    /// <param name="value">The file path to convert.</param>
    /// <returns>The TOC-relative path.</returns>
    public static string? ToTocRelativePath(this string value)
    {
        try
        {
            // If there is no file extension, this is a relative URL,
            // not a file path. Return the value unchanged.
            var fileExtension = Path.GetExtension(value);
            if (string.IsNullOrEmpty(fileExtension))
            {
                return value;
            }

            var folderName = Directory.GetParent(value)?.Name;
            var fileName = Path.GetFileName(value);
            return string.IsNullOrEmpty(folderName) || string.IsNullOrEmpty(fileName) ?
                null : $"../../{folderName}/{fileName}";
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a camelCase compound word into a sentence-cased word or words.
    /// </summary>
    /// <param name="value">The camelCase string to convert.</param>
    /// <returns>The converted string.</returns>
    public static string SplitCamelCaseToSentenceCase(this string value)
    {
        var matches = CamelCaseRegex().Matches(value);

        var builder = new StringBuilder();
        foreach (Match match in matches.Cast<Match>())
        {
            // Capitalize the first word
            if (builder.Length <= 0)
            {
                builder.Append(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(match.Value));
            }
            else
            {
                builder.Append(' ').Append(match.Value.ToLower());
            }
        }

        return ApplyOverrides(builder.ToString());
    }

    /// <summary>
    /// Checks if the string is equal to the resource name or the full-qualified resource name.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="resource">The <see cref="Resource"/> to compare against.</param>
    /// <returns>True if the string matches.</returns>
    public static bool MatchesResource(this string value, Resource resource)
    {
        return value.IsEqualIgnoringCase(resource.Name) ||
            value.IsEqualIgnoringCase($"{resource.GraphNamespace}.{resource.Name}");
    }

    /// <summary>
    /// Create a <see cref="Resource"/> from a resource name string.
    /// </summary>
    /// <param name="value">A resource name string.</param>
    /// <returns>A <see cref="Resource"/>.</returns>
    public static Resource ToResource(this string value)
    {
        if (value.Contains('.'))
        {
            return new Resource(
                Path.GetExtension(value).TrimStart('.'),
                Path.GetFileNameWithoutExtension(value),
                string.Empty,
                null,
                false,
                false);
        }

        return new Resource(
            value,
            "microsoft.graph",
            string.Empty,
            null,
            false,
            false);
    }

    /// <summary>
    /// Resolves a relative path against a file path and returns the full resulting path.
    /// </summary>
    /// <param name="value">A relative path.</param>
    /// <param name="file">The path to a file to resolve the relative path against.</param>
    /// <returns>A full path.</returns>
    public static string FullPathRelativeToFile(this string value, string file)
    {
        // if (value.Contains('#'))
        // {

        // }
        var parentDirectory = Path.GetDirectoryName(file) ?? string.Empty;
        var fullPath = Path.GetFullPath(Path.Combine(parentDirectory, value));
        return fullPath;
    }

    /// <summary>
    /// Removes any anchor from a URL or file path.
    /// </summary>
    /// <param name="value">The URL or file path to trim.</param>
    /// <returns>The URL or file path with the anchor removed.</returns>
    public static string TrimAnchor(this string value)
    {
        if (value.Contains('#'))
        {
            return value.Split('#')[0];
        }

        return value;
    }

    private static string ApplyOverrides(string value)
    {
        if (termOverrides != null)
        {
            foreach (var termOverride in termOverrides)
            {
                var termRegex = termOverride.CaseSensitive ?
                    new Regex($"(?<=^|\\s){termOverride.Term}(?=\\s|$)") :
                    new Regex($"(?<=^|\\s){termOverride.Term}(?=\\s|$)", RegexOptions.IgnoreCase);
                value = termRegex.Replace(value, termOverride.Override ?? string.Empty);
            }
        }

        return value;
    }

    [GeneratedRegex("^doc_type:\\s*\"?(?'docType'[a-zA-Z0-9]+)", RegexOptions.Multiline)]
    private static partial Regex DocTypeFromYamlRegex();

    [GeneratedRegex("^title:\\s*[\"']?(?'resourceName'[a-zA-Z0-9_]+)\\s+((resource|complex)\\s+type|facet)", RegexOptions.Multiline)]
    private static partial Regex TitleFromYamlRegex();

    [GeneratedRegex("^#\\s*(?'resourceName'[a-zA-Z0-9_]+)\\s+((resource|complex)\\s+type|facet)", RegexOptions.Multiline)]
    private static partial Regex TitleFromH1Regex();

    [GeneratedRegex("^\\s*namespace:\\s*(?'namespace'[\\w.]*)\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)]
    private static partial Regex NamespaceLineRegex();

    [GeneratedRegex("[A-Z][a-z]+|[a-z]+|[A-Z]+(?![a-z])|\\d+")]
    private static partial Regex CamelCaseRegex();
}
