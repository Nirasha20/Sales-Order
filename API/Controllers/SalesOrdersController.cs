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

        // TEST endpoint - Insert sample data to verify MySQL connection
        [HttpPost("test-data")]
        public async Task<IActionResult> InsertTestData()
        {
            try
            {
                // Check if test data already exists
                var existingClient = await _db.Clients.FirstOrDefaultAsync(c => c.CustomerName == "Test Client");
                if (existingClient != null)
                    return BadRequest("Test data already exists");

                // Create test client
                var client = new Client { CustomerName = "Test Client", Address1 = "123 Test St", City = "Test City", State = "TS", PostCode = "12345" };
                _db.Clients.Add(client);
                await _db.SaveChangesAsync();

                // Create test item
                var item = new Item { ItemCode = "TEST001", Description = "Test Item", Price = 100.00m };
                _db.Items.Add(item);
                await _db.SaveChangesAsync();

                // Create test order
                var order = new SalesOrder
                {
                    ClientId = client.ClientId,
                    InvoiceNo = "INV-TEST-001",
                    InvoiceDate = DateTime.Now,
                    ReferenceNo = "REF001",
                    TotalExcl = 100.00m,
                    TotalTax = 10.00m,
                    TotalIncl = 110.00m,
                    CreatedAt = DateTime.Now,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail { ItemId = item.ItemId, Quantity = 1, Price = 100.00m, ExclAmount = 100.00m, TaxAmount = 10.00m, InclAmount = 110.00m }
                    }
                };
                _db.SalesOrders.Add(order);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Test data inserted successfully", clientId = client.ClientId, orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, innerError = ex.InnerException?.Message });
            }
        }

        // TEST endpoint - Verify data in database
        [HttpGet("verify-data")]
        public async Task<IActionResult> VerifyData()
        {
            try
            {
                var clientCount = await _db.Clients.CountAsync();
                var itemCount = await _db.Items.CountAsync();
                var orderCount = await _db.SalesOrders.CountAsync();

                return Ok(new
                {
                    status = "Connected to MySQL",
                    clientsCount = clientCount,
                    itemsCount = itemCount,
                    ordersCount = orderCount,
                    sampleClients = await _db.Clients.Take(5).ToListAsync(),
                    sampleOrders = await _db.SalesOrders.Include(o => o.Client).Take(5).ToListAsync()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Database connection failed", message = ex.Message });
            }
        }
    }
}