using EngineeringModel.BuildingBlocks;
using EngineeringModel.Modules.Projects.Contracts;
using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.Application;

public sealed class CreateWorkItemHandler(
    IWorkItemRepository repository,
    IProjectsCatalog projectsCatalog)
{
    public async Task<WorkItemView> HandleAsync(
        Guid projectId,
        string title,
        CancellationToken cancellationToken = default)
    {
        var project = await projectsCatalog.FindAsync(projectId, cancellationToken)
            ?? throw new ResourceNotFoundException($"Project '{projectId}' was not found.");

        if (!project.IsActive)
        {
            throw new BusinessRuleViolationException("Work items can be created only for active projects.");
        }

        var workItem = WorkItem.Create(projectId, title);
        await repository.AddAsync(workItem, cancellationToken);
        return WorkItemView.From(workItem);
    }
}
