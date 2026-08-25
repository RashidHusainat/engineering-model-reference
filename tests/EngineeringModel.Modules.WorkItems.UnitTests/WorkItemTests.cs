using EngineeringModel.BuildingBlocks;
using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.UnitTests;

[TestFixture]
public sealed class WorkItemTests
{
    [Test]
    public void Create_WithValidData_StartsOpen()
    {
        var projectId = Guid.NewGuid();

        var workItem = WorkItem.Create(projectId, "Protect architecture boundaries");

        Assert.Multiple(() =>
        {
            Assert.That(workItem.ProjectId, Is.EqualTo(projectId));
            Assert.That(workItem.Title, Is.EqualTo("Protect architecture boundaries"));
            Assert.That(workItem.Status, Is.EqualTo(WorkItemStatus.Open));
        });
    }

    [Test]
    public void Complete_WhenAlreadyCompleted_RejectsTheTransition()
    {
        var workItem = WorkItem.Create(Guid.NewGuid(), "Protect architecture boundaries");
        workItem.Complete();

        var exception = Assert.Throws<BusinessRuleViolationException>(workItem.Complete);

        Assert.That(exception!.Message, Is.EqualTo("A completed work item cannot be completed again."));
    }
}
