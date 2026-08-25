using EngineeringModel.Modules.WorkItems.Application;

namespace EngineeringModel.ArchitectureTests;

[TestFixture]
public sealed class ModuleBoundaryTests
{
    private static readonly string[] ForbiddenProjectsAssemblies =
    [
        "EngineeringModel.Modules.Projects.Domain",
        "EngineeringModel.Modules.Projects.Application",
        "EngineeringModel.Modules.Projects.Infrastructure"
    ];

    [Test]
    public void WorkItemsApplication_ConsumesProjectsOnlyThroughContracts()
    {
        var references = typeof(CreateWorkItemHandler)
            .Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToArray();

        Assert.Multiple(() =>
        {
            Assert.That(
                references,
                Does.Contain("EngineeringModel.Modules.Projects.Contracts"),
                "WorkItems.Application should collaborate with Projects through Projects.Contracts.");

            foreach (var forbiddenAssembly in ForbiddenProjectsAssemblies)
            {
                Assert.That(
                    references,
                    Does.Not.Contain(forbiddenAssembly),
                    $"WorkItems.Application must not reference {forbiddenAssembly}.");
            }
        });
    }

    [Test]
    public void ProjectsModule_DoesNotReferenceWorkItemsModule()
    {
        var projectAssemblies = new[]
        {
            typeof(EngineeringModel.Modules.Projects.Domain.Project).Assembly,
            typeof(EngineeringModel.Modules.Projects.Application.CreateProjectHandler).Assembly,
            typeof(EngineeringModel.Modules.Projects.Contracts.IProjectsCatalog).Assembly,
            typeof(EngineeringModel.Modules.Projects.Infrastructure.SqliteProjectRepository).Assembly
        };

        foreach (var assembly in projectAssemblies)
        {
            var references = assembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name)
                .Where(name => name?.StartsWith("EngineeringModel.Modules.WorkItems", StringComparison.Ordinal) == true)
                .ToArray();

            Assert.That(references, Is.Empty, $"{assembly.GetName().Name} must not reference the WorkItems module.");
        }
    }

    [Test]
    public void WorkItemsInfrastructure_DoesNotBypassProjectsContracts()
    {
        var references = typeof(EngineeringModel.Modules.WorkItems.Infrastructure.SqliteWorkItemRepository)
            .Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name?.StartsWith("EngineeringModel.Modules.Projects", StringComparison.Ordinal) == true)
            .ToArray();

        Assert.That(
            references,
            Has.All.EqualTo("EngineeringModel.Modules.Projects.Contracts"),
            "If WorkItems.Infrastructure collaborates with Projects, it may use only Projects.Contracts.");
    }

    [Test]
    public void WorkItemsDomain_DoesNotReferenceProjectsModule()
    {
        var references = typeof(EngineeringModel.Modules.WorkItems.Domain.WorkItem)
            .Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name?.StartsWith("EngineeringModel.Modules.Projects", StringComparison.Ordinal) == true)
            .ToArray();

        Assert.That(references, Is.Empty, "WorkItems.Domain must not reference the Projects module.");
    }
}
