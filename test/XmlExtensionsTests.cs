// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.Extensions;

namespace GenerateTOCTests;

/// <summary>
/// Tests for the XmlExtensions class.
/// </summary>
public class XmlExtensionsTests
{
    /// <summary>
    /// Tests that getting descendants from an XDocument succeeds.
    /// </summary>
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

    /// <summary>
    /// Tests that getting elements from an XElement succeeds.
    /// </summary>
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

    /// <summary>
    /// Tests that getting an attribute from an XElement succeeds.
    /// </summary>
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
