using System.Net;
using System.Net.Http.Json;
using EngineeringModel.Modules.Projects.Application;
using EngineeringModel.Modules.WorkItems.Application;

namespace EngineeringModel.Api.IntegrationTests;

[TestFixture]
public sealed class WorkManagementFlowTests
{
    [Test]
    public async Task WorkItem_CanBeCreatedOnlyAfterProjectIsActivated()
    {
        using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var createProjectResponse = await client.PostAsJsonAsync(
            "/api/projects/",
            new { Name = "Engineering Verification" });
        createProjectResponse.EnsureSuccessStatusCode();

        var project = await createProjectResponse.Content.ReadFromJsonAsync<ProjectView>();
        Assert.That(project, Is.Not.Null);
        Assert.That(project!.Status, Is.EqualTo("Draft"));

        var rejectedWorkItemResponse = await client.PostAsJsonAsync(
            "/api/work-items/",
            new { ProjectId = project.Id, Title = "Create architecture test" });
        Assert.That(rejectedWorkItemResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var activateProjectResponse = await client.PostAsync(
            $"/api/projects/{project.Id}/activate",
            content: null);
        activateProjectResponse.EnsureSuccessStatusCode();

        var createWorkItemResponse = await client.PostAsJsonAsync(
            "/api/work-items/",
            new { ProjectId = project.Id, Title = "Create architecture test" });
        Assert.That(createWorkItemResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var workItem = await createWorkItemResponse.Content.ReadFromJsonAsync<WorkItemView>();
        Assert.That(workItem, Is.Not.Null);
        Assert.That(workItem!.Status, Is.EqualTo("Open"));

        var completeResponse = await client.PostAsync(
            $"/api/work-items/{workItem.Id}/complete",
            content: null);
        completeResponse.EnsureSuccessStatusCode();

        var completed = await completeResponse.Content.ReadFromJsonAsync<WorkItemView>();
        Assert.That(completed, Is.Not.Null);
        Assert.That(completed!.Status, Is.EqualTo("Completed"));
    }
}
