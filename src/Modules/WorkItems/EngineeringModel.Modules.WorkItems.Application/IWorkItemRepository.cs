using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.Application;

public interface IWorkItemRepository
{
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);

    Task<WorkItem?> GetAsync(WorkItemId id, CancellationToken cancellationToken = default);

    Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default);
}
