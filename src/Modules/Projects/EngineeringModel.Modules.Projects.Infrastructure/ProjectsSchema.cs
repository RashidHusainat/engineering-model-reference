using Microsoft.Data.Sqlite;

namespace EngineeringModel.Modules.Projects.Infrastructure;

public static class ProjectsSchema
{
    public static async Task InitializeAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS projects_projects (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL,
                status INTEGER NOT NULL
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
