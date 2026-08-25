using EngineeringModel.BuildingBlocks;
using EngineeringModel.Modules.Projects.Domain;

namespace EngineeringModel.Modules.Projects.UnitTests;

[TestFixture]
public sealed class ProjectTests
{
    [Test]
    public void Create_WithValidName_StartsInDraft()
    {
        var project = Project.Create("Verification Platform");

        Assert.Multiple(() =>
        {
            Assert.That(project.Id.Value, Is.Not.EqualTo(Guid.Empty));
            Assert.That(project.Name, Is.EqualTo("Verification Platform"));
            Assert.That(project.Status, Is.EqualTo(ProjectStatus.Draft));
        });
    }

    [Test]
    public void Create_WithBlankName_RejectsTheProject()
    {
        var exception = Assert.Throws<BusinessRuleViolationException>(() => Project.Create("   "));

        Assert.That(exception!.Message, Is.EqualTo("Project name is required."));
    }

    [Test]
    public void Activate_WhenAlreadyActive_RejectsTheTransition()
    {
        var project = Project.Create("Verification Platform");
        project.Activate();

        var exception = Assert.Throws<BusinessRuleViolationException>(project.Activate);

        Assert.That(exception!.Message, Is.EqualTo("Only a draft project can be activated."));
    }
}
