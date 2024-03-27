// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.Extensions;

namespace GenerateTOCTests;

public class XmlExtensionsTests
{
    [Fact]
    public void GetDescendantsSucceeds()
    {
        // Arrange
        var document = SampleData.GetSampleCsdl();

        // Act
        var descendants = document.GetDescendants("Schema");

        // Assert
        Assert.NotNull(descendants);
        Assert.Equal(2, descendants.Count());
    }

    [Fact]
    public void GetElementsSucceeds()
    {
        // Arrange
        var element = SampleData.GetSampleSchemaElement();

        // Act
        var elements = element?.GetElements("EntityType");

        // Assert
        Assert.NotNull(elements);
        Assert.Equal(2, elements.Count());
    }

    [Fact]
    public void GetAttributeSucceeds()
    {
        // Arrange
        var element = SampleData.GetSampleSchemaElement();

        // Act
        var attribute = element?.GetAttribute("Namespace");

        // Assert
        Assert.NotNull(attribute);
        Assert.Equal("microsoft.graph", attribute);
    }
}
