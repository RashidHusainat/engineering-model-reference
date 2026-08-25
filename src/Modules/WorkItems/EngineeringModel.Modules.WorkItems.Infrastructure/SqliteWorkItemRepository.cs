using EngineeringModel.Modules.WorkItems.Application;
using EngineeringModel.Modules.WorkItems.Domain;
using Microsoft.Data.Sqlite;

namespace EngineeringModel.Modules.WorkItems.Infrastructure;

public sealed class SqliteWorkItemRepository(string connectionString) : IWorkItemRepository
{
    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO workitems_items (id, project_id, title, status)
            VALUES ($id, $projectId, $title, $status);
            """;
        command.Parameters.AddWithValue("$id", workItem.Id.Value.ToString("D"));
        command.Parameters.AddWithValue("$projectId", workItem.ProjectId.ToString("D"));
        command.Parameters.AddWithValue("$title", workItem.Title);
        command.Parameters.AddWithValue("$status", (int)workItem.Status);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<WorkItem?> GetAsync(WorkItemId id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, project_id, title, status
            FROM workitems_items
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id.Value.ToString("D"));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return WorkItem.Rehydrate(
            Guid.Parse(reader.GetString(0)),
            Guid.Parse(reader.GetString(1)),
            reader.GetString(2),
            (WorkItemStatus)reader.GetInt32(3));
    }

    public async Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE workitems_items
            SET title = $title, status = $status
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", workItem.Id.Value.ToString("D"));
        command.Parameters.AddWithValue("$title", workItem.Title);
        command.Parameters.AddWithValue("$status", (int)workItem.Status);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
