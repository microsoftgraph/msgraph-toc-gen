// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.CSDL;

namespace GenerateTOCTests;

/// <summary>
/// Tests for the Resource class.
/// </summary>
public class ResourceTests
{
    /// <summary>
    /// Tests that a resource is generated correctly for an entity without a base type.
    /// </summary>
    [Fact]
    public void ResourceGeneratesForEntityWithoutBaseType()
    {
        // Arrange
        var entityElement = SampleData.GetSampleEntityElement();
        var graphNamespace = "microsoft.graph";
        var workload = "Microsoft.Canteens";

        // Act
        var resource = Resource.CreateFromXElement(entityElement, graphNamespace, workload);

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(graphNamespace, resource.GraphNamespace);
        Assert.Equal(workload, resource.Workload);
        Assert.Equal("canteen", resource.Name);
        Assert.Null(resource.BaseType);
    }

    /// <summary>
    /// Tests that a resource is generated correctly for a hidden entity.
    /// </summary>
    [Fact]
    public void ResourceGeneratesForHiddenEntity()
    {
        // Arrange
        var entityElement = SampleData.GetSampleHiddenEntityElement();
        var graphNamespace = "microsoft.graph";
        var workload = "Microsoft.Canteens";

        // Act
        var resource = Resource.CreateFromXElement(entityElement, graphNamespace, workload);

        // Assert
        Assert.NotNull(resource);
        Assert.True(resource.IsHidden);
    }

    /// <summary>
    /// Tests that a resource is generated correctly for an entity with a base type.
    /// </summary>
    [Fact]
    public void ResourceGeneratesForEntityWithBaseType()
    {
        // Arrange
        var entityElement = SampleData.GetSampleEntityWithBaseTypeElement();
        var graphNamespace = "microsoft.graph";
        var workload = "Microsoft.Canteens";

        // Act
        var resource = Resource.CreateFromXElement(entityElement, graphNamespace, workload);

        // Assert
        Assert.NotNull(resource);
        Assert.Equal(graphNamespace, resource.GraphNamespace);
        Assert.Equal(workload, resource.Workload);
        Assert.Equal("canteen", resource.Name);
        Assert.Equal("microsoft.graph.entity", resource.BaseType);
    }
}
