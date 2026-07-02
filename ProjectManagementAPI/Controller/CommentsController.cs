using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly string _connectionString;

    public CommentsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    // GET: api/comments/task/1
    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetCommentsByTask(int taskId)
    {
        using var connection = new SqlConnection(_connectionString);
        var comments = await connection.QueryAsync<Comment>(
            "sp_GetCommentsByTask",
            new { TaskId = taskId },
            commandType: CommandType.StoredProcedure);
        return Ok(comments);
    }

    // POST: api/comments
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_CreateComment",
            new { Content = dto.Content, TaskId = dto.TaskId, UserId = dto.UserId },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Comment created successfully" });
    }

    // PUT: api/comments/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] CreateCommentDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_UpdateComment",
            new { CommentId = id, Content = dto.Content },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Comment updated successfully" });
    }

    // DELETE: api/comments/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_DeleteComment",
            new { CommentId = id },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Comment deleted successfully" });
    }
}