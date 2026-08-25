using EngineeringModel.Api;
using EngineeringModel.Modules.Projects.Application;
using EngineeringModel.Modules.Projects.Contracts;
using EngineeringModel.Modules.Projects.Infrastructure;
using EngineeringModel.Modules.WorkItems.Application;
using EngineeringModel.Modules.WorkItems.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database")
    ?? "Data Source=engineering-model.db";

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<BusinessExceptionHandler>();

builder.Services.AddScoped<IProjectRepository>(_ => new SqliteProjectRepository(connectionString));
builder.Services.AddScoped<IProjectsCatalog>(_ => new SqliteProjectsCatalog(connectionString));
builder.Services.AddScoped<CreateProjectHandler>();
builder.Services.AddScoped<ActivateProjectHandler>();
builder.Services.AddScoped<GetProjectHandler>();

builder.Services.AddScoped<IWorkItemRepository>(_ => new SqliteWorkItemRepository(connectionString));
builder.Services.AddScoped<CreateWorkItemHandler>();
builder.Services.AddScoped<CompleteWorkItemHandler>();
builder.Services.AddScoped<GetWorkItemHandler>();

var app = builder.Build();

await ProjectsSchema.InitializeAsync(connectionString);
await WorkItemsSchema.InitializeAsync(connectionString);

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new
{
    service = "Engineering Model Reference",
    architecture = "Modular Monolith",
    verification = "eng/verify.ps1"
}));

var projects = app.MapGroup("/api/projects");

projects.MapPost("/", async (
    CreateProjectRequest request,
    CreateProjectHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(request.Name, cancellationToken);
    return Results.Created($"/api/projects/{result.Id}", result);
});

projects.MapPost("/{id:guid}/activate", async (
    Guid id,
    ActivateProjectHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(id, cancellationToken);
    return Results.Ok(result);
});

projects.MapGet("/{id:guid}", async (
    Guid id,
    GetProjectHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(id, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

var workItems = app.MapGroup("/api/work-items");

workItems.MapPost("/", async (
    CreateWorkItemRequest request,
    CreateWorkItemHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(request.ProjectId, request.Title, cancellationToken);
    return Results.Created($"/api/work-items/{result.Id}", result);
});

workItems.MapPost("/{id:guid}/complete", async (
    Guid id,
    CompleteWorkItemHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(id, cancellationToken);
    return Results.Ok(result);
});

workItems.MapGet("/{id:guid}", async (
    Guid id,
    GetWorkItemHandler handler,
    CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(id, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();

public partial class Program;
