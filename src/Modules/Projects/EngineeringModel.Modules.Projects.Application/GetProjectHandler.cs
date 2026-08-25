using EngineeringModel.Modules.Projects.Domain;

namespace EngineeringModel.Modules.Projects.Application;

public sealed class GetProjectHandler(IProjectRepository repository)
{
    public async Task<ProjectView?> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await repository.GetAsync(new ProjectId(id), cancellationToken);
        return project is null ? null : ProjectView.From(project);
    }
}
