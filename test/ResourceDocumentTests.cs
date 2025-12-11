// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.Docs;

namespace GenerateTOCTests;

/// <summary>
/// Tests for the ResourceDocument class.
/// </summary>
public class ResourceDocumentTests
{
    /// <summary>
    /// Tests that a resource document loads correctly from Markdown.
    /// </summary>
    [Fact]
    public void ResourceDocumentLoadsCorrectly()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithTitleInYamlAndHeader;
        var resourceDoc = new ResourceDocument(string.Empty);

        // Act
        resourceDoc.LoadMarkdown(markdown);

        // Assert
        Assert.NotNull(resourceDoc.ResourceName);
        Assert.Equal("membersDeletedEventMessageDetail", resourceDoc.ResourceName);
        Assert.NotNull(resourceDoc.GraphNameSpace);
        Assert.Equal("microsoft.graph", resourceDoc.GraphNameSpace);
        Assert.Equal(16, resourceDoc.Methods.Count);
    }

    /// <summary>
    /// Tests that a non-resource document throws a DocTypeException when loading.
    /// </summary>
    [Fact]
    public void NonResourceDocumentDoesNotLoad()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithWrongDocType;
        var resourceDoc = new ResourceDocument(string.Empty);

        // Assert
        Assert.Throws<DocTypeException>(() =>
        {
            resourceDoc.LoadMarkdown(markdown);
        });
    }
}
