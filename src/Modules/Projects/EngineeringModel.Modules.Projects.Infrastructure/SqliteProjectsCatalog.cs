using EngineeringModel.Modules.Projects.Contracts;
using EngineeringModel.Modules.Projects.Domain;
using Microsoft.Data.Sqlite;

namespace EngineeringModel.Modules.Projects.Infrastructure;

public sealed class SqliteProjectsCatalog(string connectionString) : IProjectsCatalog
{
    public async Task<ProjectSummary?> FindAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT id, name, status
            FROM projects_projects
            WHERE id = $id;
            """;
        command.Parameters.AddWithValue("$id", projectId.ToString("D"));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var status = (ProjectStatus)reader.GetInt32(2);
        return new ProjectSummary(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            status == ProjectStatus.Active);
    }
}
