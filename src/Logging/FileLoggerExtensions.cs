// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace GenerateTOC.Logging;

/// <summary>
/// Adds methods to the <see cref="ILoggingBuilder"/> implementation to add a file logging provider.
/// </summary>
public static class FileLoggerExtensions
{
    /// <summary>
    /// Adds a <see cref="FileLoggerProvider"/> to the list of log providers.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> instance to add the provider to.</param>
    /// <param name="filePath">The path to the log file.</param>
    /// <param name="minLogLevel">The minimum log level to add to the log.</param>
    /// <returns>The factory.</returns>
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string? filePath, LogLevel minLogLevel)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(
                    (provider) =>
                    {
                        return new FileLoggerProvider(filePath, minLogLevel);
                    }));
        }

        return builder;
    }
}
