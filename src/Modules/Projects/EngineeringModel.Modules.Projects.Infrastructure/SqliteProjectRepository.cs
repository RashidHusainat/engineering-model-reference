using EngineeringModel.Modules.Projects.Application;
using EngineeringModel.Modules.Projects.Domain;
using Microsoft.Data.Sqlite;

namespace EngineeringModel.Modules.Projects.Infrastructure;

public sealed class SqliteProjectRepository(string connectionString) : IProjectRepository
{
    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO projects_projects (id, name, status)
            VALUES ($id, $name, $status);
            """;
        command.Parameters.AddWithValue("$id", project.Id.Value.ToString("D"));
        command.Parameters.AddWithValue("$name", project.Name);
        command.Parameters.AddWithValue("$status", (int)project.Status);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Project?> GetAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, status
            FROM projects_projects
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", id.Value.ToString("D"));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return Project.Rehydrate(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            (ProjectStatus)reader.GetInt32(2));
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE projects_projects
            SET name = $name, status = $status
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", project.Id.Value.ToString("D"));
        command.Parameters.AddWithValue("$name", project.Name);
        command.Parameters.AddWithValue("$status", (int)project.Status);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
