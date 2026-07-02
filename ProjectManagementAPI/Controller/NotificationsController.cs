using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly string _connectionString;

    public NotificationsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    // GET: api/notifications/user/1
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetNotificationsByUser(int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        var notifications = await connection.QueryAsync<Notification>(
            "sp_GetNotificationsByUser",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
        return Ok(notifications);
    }

    // POST: api/notifications
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_CreateNotification",
            new { Message = dto.Message, UserId = dto.UserId, TaskId = dto.TaskId },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Notification created successfully" });
    }

    // PUT: api/notifications/1/read
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_MarkNotificationAsRead",
            new { NotificationId = id },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Notification marked as read" });
    }

    // DELETE: api/notifications/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_DeleteNotification",
            new { NotificationId = id },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Notification deleted successfully" });
    }
}