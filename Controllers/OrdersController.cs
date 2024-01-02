using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZApi.DTO;
using PZApi.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PZApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly CarServiceConext _context;
        public OrdersController(CarServiceConext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _context.Orders.ToListAsync();
            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return Ok(orders);

        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("getparts/{orderId}")]
        public async Task<ActionResult<IEnumerable<Part>>> GetPartsByOrderId(int orderId)
        {
            var parts = await _context.Parts
                .Where(part => part.OrderID == orderId)
                .ToListAsync();

            if (parts == null || !parts.Any())
            {
                return NotFound("Parts not found");
            }

            var jsonSettings = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };
            var jsonResult = JsonSerializer.Serialize(parts, jsonSettings); //Returns order with its parts

            return Ok(jsonResult);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] OrderDto orderDto)
        {
            var existingCustomer = await _context.Customers.FindAsync(orderDto.CustomerId);
            if (existingCustomer == null)
            {
                return BadRequest("Invalid CustomerId");// 404 customer not found
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newOrder = new Order
            {
                ServiceName = orderDto.ServiceName,
                ServicePrice = orderDto.ServicePrice,
                CustomerId = orderDto.CustomerId,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();


            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.CustomerId }, newOrder);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
