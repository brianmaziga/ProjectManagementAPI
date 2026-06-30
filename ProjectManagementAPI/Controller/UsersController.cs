using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly string _connectionString;

    public UsersController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    // GET: api/users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        using var connection = new SqlConnection(_connectionString);
        var users = await connection.QueryAsync<User>(
            "sp_GetAllUsers",
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(users);
    }

    // POST: api/users/bulk-action
    [HttpPost("bulk-action")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUserActionDto dto)
    {
        if (dto.UserIds == null || dto.UserIds.Count == 0)
            return BadRequest(new { message = "No user IDs provided" });

        var status = dto.Action.ToLower() switch
        {
            "block" => "blocked",
            "activate" => "active",
            _ => null
        };

        if (status == null)
            return BadRequest(new { message = "Action must be 'block' or 'activate'" });

        var idTable = new DataTable();
        idTable.Columns.Add("Id", typeof(int));
        foreach (var id in dto.UserIds)
            idTable.Rows.Add(id);

        using var connection = new SqlConnection(_connectionString);
        var parameters = new DynamicParameters();
        parameters.Add("@UserIds", idTable.AsTableValuedParameter("IdListType"));
        parameters.Add("@Status", status);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "sp_BulkUpdateUserStatus",
            parameters,
            commandType: CommandType.StoredProcedure);

        return Ok(new { message = $"{rowsAffected} user(s) updated to '{status}'" });
    }

    // POST: api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_CreateUser",
            new { Name = dto.Name, Email = dto.Email, Password = dto.Password, Role = dto.Role },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "User created successfully" });
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto dto)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_UpdateUser",
            new { UserId = id, Name = dto.Name, Email = dto.Email },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "User updated successfully" });
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "sp_DeleteUser",
            new { UserId = id },
            commandType: System.Data.CommandType.StoredProcedure);
        return Ok(new { message = "User deleted successfully" });
    }
}
