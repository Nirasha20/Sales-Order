using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrderAPI.Infrastructure.Data;
using System.Data;

namespace SalesOrderAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DiagnosticsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("db")]
        public async Task<IActionResult> GetDatabaseInfo()
        {
            try
            {
                await using var connection = _db.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                await using var cmd = connection.CreateCommand();
                cmd.CommandText = @"
SELECT DATABASE() AS currentDatabase;
SELECT table_name AS tableName
FROM information_schema.tables
WHERE table_schema = DATABASE()
ORDER BY table_name;";

                var currentDatabase = (string?)null;
                var tables = new List<string>();

                await using var reader = await cmd.ExecuteReaderAsync();

                // Result set 1: current DB
                if (await reader.ReadAsync())
                    currentDatabase = reader["currentDatabase"]?.ToString();

                // Result set 2: tables
                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var tableName = reader["tableName"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(tableName))
                            tables.Add(tableName);
                    }
                }

                return Ok(new
                {
                    currentDatabase,
                    tables,
                    tableCount = tables.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }
    }
}
