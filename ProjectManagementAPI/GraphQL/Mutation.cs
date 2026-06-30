using Dapper;
using Microsoft.Data.SqlClient;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.GraphQL;

public class Mutation
{
    public async Task<string> CreateUser(
        string name, string email, string password, string role,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_CreateUser",
            new { Name = name, Email = email, Password = password, Role = role },
            commandType: System.Data.CommandType.StoredProcedure);
        return "User created successfully";
    }

    public async Task<string> CreateProject(
        string name, string? description, int ownerId,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_CreateProject",
            new { Name = name, Description = description, OwnerId = ownerId },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Project created successfully";
    }

    public async Task<string> UpdateTaskStatus(
        int taskId, string status,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_UpdateTaskStatus",
            new { TaskId = taskId, Status = status },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Task status updated";
    }
}