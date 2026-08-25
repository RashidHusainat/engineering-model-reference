using EngineeringModel.Modules.Projects.Application;
using EngineeringModel.Modules.Projects.Domain;
using EngineeringModel.Modules.WorkItems.Application;
using EngineeringModel.Modules.WorkItems.Domain;
using NetArchTest.Rules;

namespace EngineeringModel.ArchitectureTests;

[TestFixture]
public sealed class LayerDependencyTests
{
    [TestCase("EngineeringModel.Modules.Projects.Infrastructure")]
    [TestCase("EngineeringModel.Modules.Projects.Application")]
    [TestCase("EngineeringModel.Api")]
    public void ProjectsDomain_DoesNotDependOnOuterLayers(string forbiddenNamespace)
    {
        AssertNoDependency(typeof(Project).Assembly, forbiddenNamespace);
    }

    [TestCase("EngineeringModel.Modules.WorkItems.Infrastructure")]
    [TestCase("EngineeringModel.Modules.WorkItems.Application")]
    [TestCase("EngineeringModel.Api")]
    public void WorkItemsDomain_DoesNotDependOnOuterLayers(string forbiddenNamespace)
    {
        AssertNoDependency(typeof(WorkItem).Assembly, forbiddenNamespace);
    }

    [TestCase("EngineeringModel.Modules.Projects.Infrastructure")]
    [TestCase("EngineeringModel.Api")]
    public void ProjectsApplication_DoesNotDependOnOuterLayers(string forbiddenNamespace)
    {
        AssertNoDependency(typeof(CreateProjectHandler).Assembly, forbiddenNamespace);
    }

    [TestCase("EngineeringModel.Modules.WorkItems.Infrastructure")]
    [TestCase("EngineeringModel.Api")]
    public void WorkItemsApplication_DoesNotDependOnOuterLayers(string forbiddenNamespace)
    {
        AssertNoDependency(typeof(CreateWorkItemHandler).Assembly, forbiddenNamespace);
    }

    private static void AssertNoDependency(System.Reflection.Assembly assembly, string forbiddenNamespace)
    {
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(forbiddenNamespace)
            .GetResult();

        Assert.That(
            result.IsSuccessful,
            Is.True,
            $"Assembly '{assembly.GetName().Name}' must not depend on '{forbiddenNamespace}'.");
    }
}
