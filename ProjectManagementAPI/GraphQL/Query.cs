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

    public async Task<IEnumerable<Comment>> GetCommentsByTask(int taskId, [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<Comment>(
            "sp_GetCommentsByTask",
            new { TaskId = taskId },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Team>> GetTeamsByProject(int projectId, [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<Team>(
            "sp_GetTeamsByProject",
            new { ProjectId = projectId },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<TeamMember>> GetTeamMembers(int teamId, [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<TeamMember>(
            "sp_GetTeamMembers",
            new { TeamId = teamId },
            commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Notification>> GetNotificationsByUser(int userId, [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        return await connection.QueryAsync<Notification>(
            "sp_GetNotificationsByUser",
            new { UserId = userId },
            commandType: System.Data.CommandType.StoredProcedure);
    }
}