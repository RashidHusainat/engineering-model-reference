using EngineeringModel.BuildingBlocks;
using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.Application;

public sealed class CompleteWorkItemHandler(IWorkItemRepository repository)
{
    public async Task<WorkItemView> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workItem = await repository.GetAsync(new WorkItemId(id), cancellationToken)
            ?? throw new ResourceNotFoundException($"Work item '{id}' was not found.");

        workItem.Complete();
        await repository.UpdateAsync(workItem, cancellationToken);
        return WorkItemView.From(workItem);
    }
}
