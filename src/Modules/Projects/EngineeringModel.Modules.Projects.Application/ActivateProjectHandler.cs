using EngineeringModel.BuildingBlocks;
using EngineeringModel.Modules.Projects.Domain;

namespace EngineeringModel.Modules.Projects.Application;

public sealed class ActivateProjectHandler(IProjectRepository repository)
{
    public async Task<ProjectView> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await repository.GetAsync(new ProjectId(id), cancellationToken)
            ?? throw new ResourceNotFoundException($"Project '{id}' was not found.");

        project.Activate();
        await repository.UpdateAsync(project, cancellationToken);
        return ProjectView.From(project);
    }
}
