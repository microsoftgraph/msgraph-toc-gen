// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace GenerateTOC.Generation;

/// <summary>
/// Exception thrown when there is an error loading the mapping file.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MappingFileException"/> class with the specified error message.
/// </remarks>
/// <param name="message">The exception message.</param>
public class MappingFileException(string message) : Exception(message)
{
}
