// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Text.RegularExpressions;

namespace TOCScan;

/// <summary>
/// Contains RegEx patterns for finding doc entries in TOC files.
/// </summary>
public static partial class TOCEntries
{
    [GeneratedRegex("href:\\s+(?>\\.\\.\\/)*resources\\/(?'file'\\S+)", RegexOptions.Multiline)]
    public static partial Regex ResourceRegex();

    [GeneratedRegex("href:\\s+(?>\\.\\.\\/)*api\\/(?'file'\\S+)", RegexOptions.Multiline)]
    public static partial Regex ApiRegex();
}
