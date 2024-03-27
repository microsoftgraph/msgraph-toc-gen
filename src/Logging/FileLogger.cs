// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace GenerateTOC.Logging;

/// <summary>
/// A logger that writes to a file.
/// </summary>
/// <param name="name">The name of the logger.</param>
/// <param name="filePath">The path to the log file.</param>
/// <param name="minLogLevel">The minimum log level to add to the log.</param>
public sealed class FileLogger(string name, string filePath, LogLevel minLogLevel) : ILogger
{
    private string Name => name;

    private string Path => filePath;

    private SemaphoreSlim Lock => new(1);

    private LogLevel MinimumLogLevel => minLogLevel;

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull
        => default!;

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= MinimumLogLevel;
    }

    /// <inheritdoc/>
    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var line = $"{Name}[{eventId.Id}];{GetMessagePrefix(logLevel)};{formatter(state, exception)}{Environment.NewLine}";

        Lock.Wait();
        File.AppendAllText(Path, line);
        Lock.Release();
    }

    private static string GetMessagePrefix(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARNING",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRITICAL",
            _ => string.Empty,
        };
    }
}
