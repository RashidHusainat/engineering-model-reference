using EngineeringModel.BuildingBlocks;

namespace EngineeringModel.Modules.WorkItems.Domain;

public sealed class WorkItem
{
    private WorkItem(WorkItemId id, Guid projectId, string title, WorkItemStatus status)
    {
        Id = id;
        ProjectId = projectId;
        Title = title;
        Status = status;
    }

    public WorkItemId Id { get; }

    public Guid ProjectId { get; }

    public string Title { get; }

    public WorkItemStatus Status { get; private set; }

    public static WorkItem Create(Guid projectId, string title)
    {
        if (projectId == Guid.Empty)
        {
            throw new BusinessRuleViolationException("Project id is required.");
        }

        return new WorkItem(WorkItemId.New(), projectId, ValidateTitle(title), WorkItemStatus.Open);
    }

    public static WorkItem Rehydrate(Guid id, Guid projectId, string title, WorkItemStatus status)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessRuleViolationException("Work item id cannot be empty.");
        }

        if (projectId == Guid.Empty)
        {
            throw new BusinessRuleViolationException("Project id is required.");
        }

        return new WorkItem(new WorkItemId(id), projectId, ValidateTitle(title), status);
    }

    public void Complete()
    {
        if (Status == WorkItemStatus.Completed)
        {
            throw new BusinessRuleViolationException("A completed work item cannot be completed again.");
        }

        Status = WorkItemStatus.Completed;
    }

    private static string ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessRuleViolationException("Work item title is required.");
        }

        var normalized = title.Trim();
        if (normalized.Length > 200)
        {
            throw new BusinessRuleViolationException("Work item title cannot exceed 200 characters.");
        }

        return normalized;
    }
}
