// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace GenerateTOC.Logging;

/// <summary>
/// Contains methods to create a logger for the app.
/// </summary>
public static class OutputLoggerFactory
{
    /// <summary>
    /// Gets an <see cref="ILogger"/> instance.
    /// </summary>
    /// <typeparam name="T">The type to use to create the <see cref="ILogger"/>.</typeparam>
    /// <param name="logFile">The path to the log file.</param>
    /// <returns>The <see cref="ILogger"/> that was created.</returns>
    public static ILogger GetLogger<T>(string? logFile)
    {
        DeleteFileIfExists(logFile);

        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .ClearProviders()
                .SetMinimumLevel(LogLevel.Information)
                .AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                })
                .AddFile(logFile, LogLevel.Information);
        });

        return loggerFactory.CreateLogger<T>();
    }

    private static void DeleteFileIfExists(string? logFile)
    {
        if (!string.IsNullOrEmpty(logFile) && File.Exists(logFile))
        {
            File.Delete(logFile);
        }
    }
}
