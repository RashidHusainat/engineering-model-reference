namespace EngineeringModel.Modules.Projects.Domain;

public readonly record struct ProjectId(Guid Value)
{
    public static ProjectId New() => new(Guid.NewGuid());
}
