// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace GenerateTOC.Docs;

/// <summary>
/// Represents a collection of Markdown documents for an API.
/// </summary>
public class DocSet
{
    private readonly ILogger logger;

    private DocSet(string docsRoot, ILogger logger)
    {
        RootDirectory = docsRoot;
        ResourceDocuments = [];
        this.logger = logger;
    }

    /// <summary>
    /// Gets the documents contained in the collection.
    /// </summary>
    public List<ResourceDocument> ResourceDocuments { get; private set; }

    /// <summary>
    /// Gets the root directory containing the documents.
    /// </summary>
    public string RootDirectory { get; private set; }

    /// <summary>
    /// Creates a <see cref="DocSet"/> from the files contained in a directory.
    /// </summary>
    /// <param name="docsRoot">The path to the directory to create the <see cref="DocSet"/> from.</param>
    /// <param name="logger">An <see cref="ILogger"/> instance to use for output.</param>
    /// <returns>A task that represents the asynchronous create operation. The task result contains the created <see cref="DocSet"/>.</returns>
    public static async Task<DocSet> CreateFromDirectory(string docsRoot, ILogger logger)
    {
        var docSet = new DocSet(docsRoot, logger);
        await docSet.LoadDirectory();
        return docSet;
    }

    /// <summary>
    /// Loads the Markdown files in the root directory into the <see cref="DocSet"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    public async Task LoadDirectory()
    {
        var markdownFiles = Directory.EnumerateFiles(RootDirectory, "*.md");
        if (markdownFiles != null)
        {
            foreach (var file in markdownFiles)
            {
                var resourceDoc = await ResourceDocument.CreateFromMarkdownFile(file, logger);
                if (resourceDoc != null)
                {
                    ResourceDocuments.Add(resourceDoc);
                }
            }
        }
    }
}
