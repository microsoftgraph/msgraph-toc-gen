// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.CSDL;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GenerateTOCTests;

public class WorkloadCollectionTests
{
    private readonly ILogger logger = new NullLoggerFactory().CreateLogger("test");

    [Fact]
    public void WorkloadGeneratesCorrectly()
    {
        // Arrange
        var collection = new WorkloadCollection("../../../data", logger);

        // Act
        var workload = collection.CreateWorkloadFromFolder("../../../data/Microsoft.Test", "beta");
        var solutionsRoot = workload.Resources.SingleOrDefault(r => r.Name == "solutionsRoot");

        // Assert
        Assert.Equal("Microsoft.Test", workload.Id);
        Assert.NotNull(workload.Csdl);
        Assert.Equal(7, workload.Resources.Count);

        Assert.NotNull(solutionsRoot);
        Assert.Equal("solutionsRoot", solutionsRoot.Name);
        Assert.Equal("microsoft.graph", solutionsRoot.GraphNamespace);
        Assert.Equal("Microsoft.Test", solutionsRoot.Workload);
        Assert.Null(solutionsRoot.BaseType);
    }
}
