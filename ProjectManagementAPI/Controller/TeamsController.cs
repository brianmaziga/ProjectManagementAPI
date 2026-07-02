using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectManagementAPI.Models;
using System.Data;

namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly string _connectionString;

    public TeamsController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    // GET: api/teams/project/1
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetTeamsByProject(int projectId)
    {
        using var connection = new SqlConnection(_connectionString);
        var teams = await connection.QueryAsync<Team>(
            "sp_GetTeamsByProject",
            new { ProjectId = projectId },
            commandType: CommandType.StoredProcedure);
        return Ok(teams);
    }

    // POST: api/teams
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_CreateTeam",
            new { Name = dto.Name, ProjectId = dto.ProjectId, CreatedBy = dto.CreatedBy },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Team created successfully" });
    }

    // PUT: api/teams/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] CreateTeamDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_UpdateTeam",
            new { TeamId = id, Name = dto.Name },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Team updated successfully" });
    }

    // DELETE: api/teams/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_DeleteTeam",
            new { TeamId = id },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Team deleted successfully" });
    }

    // POST: api/teams/members
    [HttpPost("members")]
    public async Task<IActionResult> AddTeamMember([FromBody] AddTeamMemberDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_AddTeamMember",
            new { TeamId = dto.TeamId, UserId = dto.UserId },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Member added successfully" });
    }

    // GET: api/teams/1/members
    [HttpGet("{teamId}/members")]
    public async Task<IActionResult> GetTeamMembers(int teamId)
    {
        using var connection = new SqlConnection(_connectionString);
        var members = await connection.QueryAsync<TeamMember>(
            "sp_GetTeamMembers",
            new { TeamId = teamId },
            commandType: CommandType.StoredProcedure);
        return Ok(members);
    }

    // DELETE: api/teams/1/members/5
    [HttpDelete("{teamId}/members/{userId}")]
    public async Task<IActionResult> RemoveTeamMember(int teamId, int userId)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_RemoveTeamMember",
            new { TeamId = teamId, UserId = userId },
            commandType: CommandType.StoredProcedure);
        return Ok(new { message = "Member removed successfully" });
    }
}