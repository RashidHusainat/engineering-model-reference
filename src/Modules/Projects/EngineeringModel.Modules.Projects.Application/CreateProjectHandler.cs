using EngineeringModel.Modules.Projects.Domain;

namespace EngineeringModel.Modules.Projects.Application;

public sealed class CreateProjectHandler(IProjectRepository repository)
{
    public async Task<ProjectView> HandleAsync(string name, CancellationToken cancellationToken = default)
    {
        var project = Project.Create(name);
        await repository.AddAsync(project, cancellationToken);
        return ProjectView.From(project);
    }
}
