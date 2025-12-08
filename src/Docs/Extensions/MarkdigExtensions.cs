// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Immutable;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace GenerateTOC.Docs.Extensions;

/// <summary>
/// Contains extensions for Markdig classes.
/// </summary>
public static class MarkdigExtensions
{
    /// <summary>
    /// Checks if the heading text in a <see cref="HeadingBlock"/> equals a given value.
    /// </summary>
    /// <param name="block">The <see cref="HeadingBlock"/> instance to check.</param>
    /// <param name="value">The value to check against the heading text.</param>
    /// <returns>A value indicating whether the values are equal.</returns>
    public static bool TextEquals(this HeadingBlock block, string value)
    {
        var blockText = block.Inline?.FirstChild?.ToString() ?? string.Empty;
        return string.Compare(blockText, value, StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    /// <summary>
    /// Gets the text value of a <see cref="HeadingBlock"/> object.
    /// </summary>
    /// <param name="block">The <see cref="HeadingBlock"/> object.</param>
    /// <returns>The text value of the <see cref="HeadingBlock"/>.</returns>
    public static string TextValue(this HeadingBlock block)
    {
        return block.Inline?.FirstChild?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Gets the text value of a <see cref="LiteralInline"/> object.
    /// </summary>
    /// <param name="literal">The <see cref="LiteralInline"/> object.</param>
    /// <returns>The text value of the <see cref="LiteralInline"/>.</returns>
    public static string TextValue(this LiteralInline literal)
    {
        return literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length);
    }

    /// <summary>
    /// Gets a list of column headings from a <see cref="Table"/>.
    /// </summary>
    /// <param name="table">The <see cref="Table"/>.</param>
    /// <returns>A list of headings.</returns>
    public static List<string> GetColumnHeadings(this Table table)
    {
        // First row is header row
        var headerRow = table.ToImmutableList()[0];
        var headerCells = headerRow.Descendants<TableCell>().ToImmutableList();
        var headings = new List<string>();

        foreach (var headerCell in headerCells)
        {
            var text = headerCell.Descendants<LiteralInline>().FirstOrDefault()?.TextValue();
            if (!string.IsNullOrEmpty(text))
            {
                headings.Add(text);
            }
        }

        return headings;
    }

    /// <summary>
    /// Parses a <see cref="TableCell"/> as a Method cell in a Methods table.
    /// </summary>
    /// <param name="cell">The <see cref="TableCell"/>.</param>
    /// <param name="heading">The subheading this Methods table was found under, if any.</param>
    /// <returns>The method title and link.</returns>
    public static MethodLink? ParseMethodCell(this TableCell cell, string? heading)
    {
        // Method cell should contain a link.
        var linkToMethodDoc = cell.Descendants<LinkInline>().FirstOrDefault();
        if (linkToMethodDoc == null)
        {
            return null;
        }

        var url = linkToMethodDoc.Url;
        var linkText = linkToMethodDoc.Descendants<LiteralInline>().First();
        var text = linkText.TextValue();

        return string.IsNullOrEmpty(url) ? null : new MethodLink(text, url, heading);
    }

    /// <summary>
    /// Gets the text value of a <see cref="TableCell"/> object.
    /// </summary>
    /// <param name="cell">The <see cref="TableCell"/> object.</param>
    /// <returns>The text value of the <see cref="TableCell"/>.</returns>
    public static string? TextValue(this TableCell cell)
    {
        var cellText = cell.Descendants<LiteralInline>().FirstOrDefault(l => l.Content.Length > 0);
        return cellText?.TextValue();
    }
}
