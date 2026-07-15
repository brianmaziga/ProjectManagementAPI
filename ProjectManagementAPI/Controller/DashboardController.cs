using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace ProjectManagementAPI.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{

    private readonly string _connectionString;


    public DashboardController(IConfiguration config)
    {
        _connectionString =
            config.GetConnectionString("DefaultConnection")!;
    }



    // GET: api/dashboard/stats

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {

        using var connection =
            new SqlConnection(_connectionString);


        var stats = await connection.QueryFirstAsync(
            "sp_GetDashboardStats",
            commandType: CommandType.StoredProcedure
        );


        return Ok(stats);

    }

}