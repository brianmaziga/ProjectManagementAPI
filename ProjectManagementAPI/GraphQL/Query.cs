using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.GraphQL;

public class Query
{
    public async Task<IEnumerable<User>> GetUsers([Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<User>(
            "sp_GetAllUsers", commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Project>> GetProjects([Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<Project>(
            "sp_GetAllProjects", commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByProject(int projectId, [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<TaskItem>(
            "sp_GetTasksByProject",
            new { ProjectId = projectId },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}