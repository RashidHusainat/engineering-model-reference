using EngineeringModel.Modules.WorkItems.Domain;

namespace EngineeringModel.Modules.WorkItems.Application;

public sealed record WorkItemView(Guid Id, Guid ProjectId, string Title, string Status)
{
    internal static WorkItemView From(WorkItem workItem) =>
        new(workItem.Id.Value, workItem.ProjectId, workItem.Title, workItem.Status.ToString());
}
