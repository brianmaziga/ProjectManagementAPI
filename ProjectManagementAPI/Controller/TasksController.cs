using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectManagementAPI.Models;
namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly string _connectionString;
    public TasksController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }
    // GET: api/tasks
    [HttpGet]
    public async Task<IActionResult> GetAllTasks()
    {
        using var connection = new SqlConnection(_connectionString);
        var tasks = await connection.QueryAsync<TaskItem>(
            "sp_GetAllTasks",
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(tasks);
    }
    // GET: api/tasks/project/5
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetTasksByProject(int projectId)
    {
        using var connection = new SqlConnection(_connectionString);
        var tasks = await connection.QueryAsync<TaskItem>(
            "sp_GetTasksByProject",
            new { ProjectId = projectId },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(tasks);
    }
    // POST: api/tasks
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_CreateTask",
            new { Title = dto.Title, Description = dto.Description, ProjectId = dto.ProjectId, AssignedTo = dto.AssignedTo, DueDate = dto.DueDate },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "Task created successfully" });
    }
    // PUT: api/tasks/5/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_UpdateTaskStatus",
            new { TaskId = id, Status = dto.Status },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "Task status updated successfully" });
    }
    // DELETE: api/tasks/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_DeleteTask",
            new { TaskId = id },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "Task deleted successfully" });
    }
}