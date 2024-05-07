// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Xml.Linq;
using GenerateTOC.Extensions;
using Microsoft.Extensions.Logging;

namespace GenerateTOC.CSDL;

/// <summary>
/// Loads CSDL files from the AGS workloads folder.
/// </summary>
/// <param name="csdlFolder">The path to the workloads folder.</param>
/// <param name="logger">The logger for output.</param>
public class WorkloadCollection(string csdlFolder, ILogger logger)
{
    /// <summary>
    /// Gets the workloads.
    /// </summary>
    public List<Workload> Workloads { get; } = [];

    /// <summary>
    /// Gets the resources.
    /// </summary>
    public List<Resource> Resources
    {
        get
        {
            var resources = new List<Resource>();

            foreach (var workload in Workloads)
            {
                resources.AddRange(workload.Resources);
            }

            return resources;
        }
    }

    private string CsdlFolder => csdlFolder;

    private ILogger Logger => logger;

    /// <summary>
    /// Gets resources from a namespace.
    /// </summary>
    /// <param name="namespaceElement">The <see cref="XElement"/> that represents the namespace.</param>
    /// <param name="workload">The workload ID.</param>
    /// <returns>A list of resources.</returns>
    public static List<Resource>? GetResourcesFromNamespace(XElement? namespaceElement, string workload)
    {
        if (namespaceElement == null)
        {
            return null;
        }

        var graphNamespace = namespaceElement.GetAttribute("Namespace") ?? "microsoft.graph";
        if (!graphNamespace.StartsWith("microsoft.graph.", StringComparison.InvariantCultureIgnoreCase) ||
            graphNamespace.IsEqualIgnoringCase("microsoft.graph.analytics"))
        {
            graphNamespace = "microsoft.graph";
        }

        // Get all EntityType elements in this namespace
        var entityTypes = namespaceElement.GetElements("EntityType");
        var complexTypes = namespaceElement.GetElements("ComplexType");

        if ((entityTypes == null || !entityTypes.Any()) &&
            (complexTypes == null || !complexTypes.Any()))
        {
            return null;
        }

        var resources = new List<Resource>();
        foreach (var entityType in entityTypes ?? [])
        {
            if (entityType.BelongsInWorkload())
            {
                var resource = Resource.CreateFromXElement(entityType, graphNamespace, workload);
                if (resource != null)
                {
                    resources.Add(resource);
                }
            }
        }

        foreach (var complexType in complexTypes ?? [])
        {
            var resource = Resource.CreateFromXElement(complexType, graphNamespace, workload);
            if (resource != null)
            {
                resources.Add(resource);
            }
        }

        return resources;
    }

    /// <summary>
    /// Loads workload data from the CSDLs in the workload folder.
    /// </summary>
    /// <param name="version">The API version to load.</param>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    public async Task ParseWorkloadsAsync(string version = "v1.0")
    {
        // Get the list of subdirectories in the workloads folder.
        var workloadFolders = Directory.EnumerateDirectories(CsdlFolder);

        foreach (var workloadFolder in workloadFolders)
        {
            if (workloadFolder.EndsWith("/Ags") || workloadFolder.EndsWith("\\Ags"))
            {
                continue;
            }

            Workloads.Add(CreateWorkloadFromFolder(workloadFolder, version));
        }

        await Task.FromResult(0);
    }

    /// <summary>
    /// Creates an instance of the <see cref="Workload"/> class from a workload folder.
    /// </summary>
    /// <param name="workloadFolder">The path to the workload folder.</param>
    /// <param name="version">The API version to generate for.</param>
    /// <returns>An instance of <see cref="Workload"/>.</returns>
    internal Workload CreateWorkloadFromFolder(string workloadFolder, string version)
    {
        var workload = new Workload
        {
            Id = Path.GetFileName(workloadFolder),
        };

        Logger.LogInformation("Processing {workload}", Path.GetFileName(workload.Id));

        var csdlFilePath = Path.Join(workloadFolder, $"override/schema-Prod-{version}.csdl");
        if (File.Exists(csdlFilePath))
        {
            workload.Csdl = XDocument.Load(csdlFilePath);

            var namespaceElements = workload.Csdl.GetDescendants("Schema");

            foreach (var namespaceElement in namespaceElements)
            {
                var resources = GetResourcesFromNamespace(namespaceElement, workload.Id);
                if (resources != null)
                {
                    workload.Resources.AddRange(resources);
                }
            }
        }

        return workload;
    }
}
