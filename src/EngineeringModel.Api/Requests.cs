namespace EngineeringModel.Api;

public sealed record CreateProjectRequest(string Name);

public sealed record CreateWorkItemRequest(Guid ProjectId, string Title);
