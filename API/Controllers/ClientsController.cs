using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrderAPI.Infrastructure.Data;

namespace SalesOrderAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ClientsController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Clients.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _db.Clients.FindAsync(id));
    }
}