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

    public async Task<string> CreateTask(
        string title, string? description, int projectId, int assignedTo, DateTime? dueDate,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_CreateTask",
            new { Title = title, Description = description, ProjectId = projectId, AssignedTo = assignedTo, DueDate = dueDate },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Task created successfully";
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

    public async Task<string> CreateComment(
        string content, int taskId, int userId,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_CreateComment",
            new { Content = content, TaskId = taskId, UserId = userId },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Comment created successfully";
    }

    public async Task<string> CreateTeam(
        string name, int projectId, int createdBy,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_CreateTeam",
            new { Name = name, ProjectId = projectId, CreatedBy = createdBy },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Team created successfully";
    }

    public async Task<string> AddTeamMember(
        int teamId, int userId,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_AddTeamMember",
            new { TeamId = teamId, UserId = userId },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Member added successfully";
    }

    public async Task<string> CreateNotification(
        string message, int userId, int taskId,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_CreateNotification",
            new { Message = message, UserId = userId, TaskId = taskId },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Notification created successfully";
    }

    public async Task<string> MarkNotificationAsRead(
        int notificationId, int userId,
        [Service] IConfiguration config)
    {
        using var connection = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        await connection.ExecuteAsync(
            "sp_MarkNotificationAsRead",
            new { NotificationId = notificationId },
            commandType: System.Data.CommandType.StoredProcedure);
        return "Notification marked as read";
    }
}