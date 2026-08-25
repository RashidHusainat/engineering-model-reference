namespace EngineeringModel.Modules.Projects.Contracts;

public interface IProjectsCatalog
{
    Task<ProjectSummary?> FindAsync(Guid projectId, CancellationToken cancellationToken = default);
}

public sealed record ProjectSummary(Guid Id, string Name, bool IsActive);
