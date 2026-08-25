using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.Application;

public sealed class GetWorkItemHandler(IWorkItemRepository repository)
{
    public async Task<WorkItemView?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workItem = await repository.GetAsync(new WorkItemId(id), cancellationToken);
        return workItem is null ? null : WorkItemView.From(workItem);
    }
}
