using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesOrderAPI.Infrastructure.Data;
using SalesOrderAPI.Domain.Entities;

namespace SalesOrderAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SalesOrdersController(AppDbContext db) { _db = db; }

        // GET all orders
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _db.SalesOrders
                .Include(o => o.Client)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Item)
                .ToListAsync();
            return Ok(orders);
        }

        // GET single order
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _db.SalesOrders
                .Include(o => o.Client)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        // POST create new order
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SalesOrder order)
        {
            order.CreatedAt = DateTime.Now;
            _db.SalesOrders.Add(order);
            await _db.SaveChangesAsync();
            return Ok(order);
        }

        // PUT update existing order
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesOrder order)
        {
            var existing = await _db.SalesOrders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existing == null) return NotFound();

            // Update header
            existing.ClientId = order.ClientId;
            existing.InvoiceNo = order.InvoiceNo;
            existing.InvoiceDate = order.InvoiceDate;
            existing.ReferenceNo = order.ReferenceNo;
            existing.TotalExcl = order.TotalExcl;
            existing.TotalTax = order.TotalTax;
            existing.TotalIncl = order.TotalIncl;

            // Remove old details and add new ones
            _db.OrderDetails.RemoveRange(existing.OrderDetails);
            existing.OrderDetails = order.OrderDetails;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }
    }
}