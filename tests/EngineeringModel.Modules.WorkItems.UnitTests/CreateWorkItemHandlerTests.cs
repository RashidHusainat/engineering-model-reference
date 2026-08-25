using EngineeringModel.BuildingBlocks;
using EngineeringModel.Modules.Projects.Contracts;
using EngineeringModel.Modules.WorkItems.Application;
using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.UnitTests;

[TestFixture]
public sealed class CreateWorkItemHandlerTests
{
    [Test]
    public void Handle_WhenProjectIsNotActive_RejectsCreation()
    {
        var projectId = Guid.NewGuid();
        var catalog = new StubProjectsCatalog(new ProjectSummary(projectId, "Draft Project", false));
        var repository = new InMemoryWorkItemRepository();
        var handler = new CreateWorkItemHandler(repository, catalog);

        var exception = Assert.ThrowsAsync<BusinessRuleViolationException>(async () =>
            await handler.HandleAsync(projectId, "Should not be created"));

        Assert.Multiple(() =>
        {
            Assert.That(exception!.Message, Is.EqualTo("Work items can be created only for active projects."));
            Assert.That(repository.Items, Is.Empty);
        });
    }

    private sealed class StubProjectsCatalog(ProjectSummary project) : IProjectsCatalog
    {
        public Task<ProjectSummary?> FindAsync(Guid projectId, CancellationToken cancellationToken = default) =>
            Task.FromResult<ProjectSummary?>(projectId == project.Id ? project : null);
    }

    private sealed class InMemoryWorkItemRepository : IWorkItemRepository
    {
        public List<WorkItem> Items { get; } = [];

        public Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default)
        {
            Items.Add(workItem);
            return Task.CompletedTask;
        }

        public Task<WorkItem?> GetAsync(WorkItemId id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.SingleOrDefault(item => item.Id == id));

        public Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
