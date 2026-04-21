using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrderAPI.Infrastructure.Data;

namespace SalesOrderAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ItemsController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _db.Items.ToListAsync());
    }
}