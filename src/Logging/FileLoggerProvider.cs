// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace GenerateTOC.Logging;

/// <summary>
/// Logger provider for the <see cref="FileLogger"/> class.
/// </summary>
/// <param name="filePath">The path to the log file.</param>
/// <param name="minLogLevel">The minimum log level to add to the log.</param>
public sealed class FileLoggerProvider(string filePath, LogLevel minLogLevel) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, FileLogger> loggers =
        new(StringComparer.OrdinalIgnoreCase);

    private string Path => filePath;

    private LogLevel MinimumLogLevel => minLogLevel;

    /// <inheritdoc/>
    public ILogger CreateLogger(string categoryName) =>
        loggers.GetOrAdd(categoryName, new FileLogger(categoryName, Path, MinimumLogLevel));

    /// <inheritdoc/>
    public void Dispose()
    {
        loggers.Clear();
    }
}
