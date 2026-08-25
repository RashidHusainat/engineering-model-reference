using EngineeringModel.Modules.Projects.Domain;

namespace EngineeringModel.Modules.Projects.Application;

public sealed record ProjectView(Guid Id, string Name, string Status)
{
    internal static ProjectView From(Project project) =>
        new(project.Id.Value, project.Name, project.Status.ToString());
}
