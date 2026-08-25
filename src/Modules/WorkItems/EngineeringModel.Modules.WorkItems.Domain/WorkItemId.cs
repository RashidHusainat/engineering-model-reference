namespace EngineeringModel.Modules.WorkItems.Domain;

public readonly record struct WorkItemId(Guid Value)
{
    public static WorkItemId New() => new(Guid.NewGuid());
}
