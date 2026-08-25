using Microsoft.Data.Sqlite;

namespace EngineeringModel.Modules.WorkItems.Infrastructure;

public static class WorkItemsSchema
{
    public static async Task InitializeAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS workitems_items (
                id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                title TEXT NOT NULL,
                status INTEGER NOT NULL
            );
            """;

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
