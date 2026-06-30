using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly string _connectionString;

    public ProjectsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    // GET: api/projects
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        using var connection = new SqlConnection(_connectionString);
        var projects = await connection.QueryAsync<Project>(
            "sp_GetAllProjects",
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(projects);
    }

    // POST: api/projects
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_CreateProject",
            new { Name = dto.Name, Description = dto.Description, OwnerId = dto.OwnerId },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "Project created successfully" });
    }

    // PUT: api/projects/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] CreateProjectDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_UpdateProject",
            new { ProjectId = id, Name = dto.Name, Description = dto.Description },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "Project updated successfully" });
    }

    // DELETE: api/projects/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_DeleteProject",
            new { ProjectId = id },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "Project deleted successfully" });
    }
}