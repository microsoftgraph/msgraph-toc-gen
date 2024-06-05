// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace GenerateTOC;

/// <summary>
/// Event IDs for logging.
/// </summary>
public static class GenerationEventId
{
    /// <summary>
    /// A workload ID had no matching CSDL file.
    /// </summary>
    public static readonly EventId NoCsdl = 1;

    /// <summary>
    /// Could not find document for a resource.
    /// </summary>
    public static readonly EventId NoDocument = 2;

    /// <summary>
    /// The mapping file provided is not valid.
    /// </summary>
    public static readonly EventId InvalidMappingFile = 3;

    /// <summary>
    /// An error occurred while parsing a Markdown resource document.
    /// </summary>
    public static readonly EventId ResourceDocParseError = 4;

    /// <summary>
    /// Multiple documents found for a resource.
    /// </summary>
    public static readonly EventId MultipleDocsFound = 5;

    /// <summary>
    /// Resource is not found in specified workload.
    /// </summary>
    public static readonly EventId ResourceNotFound = 6;

    /// <summary>
    /// The doc_type YAML value for the document is the wrong type.
    /// </summary>
    public static readonly EventId WrongDocType = 7;

    /// <summary>
    /// The file path for a method link in a resource document is invalid.
    /// </summary>
    public static readonly EventId InvalidMethodLink = 8;
}
