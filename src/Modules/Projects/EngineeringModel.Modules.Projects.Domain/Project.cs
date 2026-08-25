using EngineeringModel.BuildingBlocks;

namespace EngineeringModel.Modules.Projects.Domain;

public sealed class Project
{
    private Project(ProjectId id, string name, ProjectStatus status)
    {
        Id = id;
        Name = name;
        Status = status;
    }

    public ProjectId Id { get; }

    public string Name { get; }

    public ProjectStatus Status { get; private set; }

    public static Project Create(string name)
    {
        var normalizedName = ValidateName(name);
        return new Project(ProjectId.New(), normalizedName, ProjectStatus.Draft);
    }

    public static Project Rehydrate(Guid id, string name, ProjectStatus status)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessRuleViolationException("Project id cannot be empty.");
        }

        return new Project(new ProjectId(id), ValidateName(name), status);
    }

    public void Activate()
    {
        if (Status != ProjectStatus.Draft)
        {
            throw new BusinessRuleViolationException("Only a draft project can be activated.");
        }

        Status = ProjectStatus.Active;
    }

    public void Close()
    {
        if (Status != ProjectStatus.Active)
        {
            throw new BusinessRuleViolationException("Only an active project can be closed.");
        }

        Status = ProjectStatus.Closed;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleViolationException("Project name is required.");
        }

        var normalized = name.Trim();
        if (normalized.Length > 120)
        {
            throw new BusinessRuleViolationException("Project name cannot exceed 120 characters.");
        }

        return normalized;
    }
}
