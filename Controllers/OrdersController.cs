using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZApi.DTO;
using PZApi.Models;
using static NuGet.Packaging.PackagingConstants;
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
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("getcustomerorder")]
        public async Task<ActionResult<IEnumerable<Order>>> GetCustomerOrder(int customerId)
        {
            var customerOrders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Parts)
                .ToListAsync();

            if (customerOrders == null || customerOrders.Count == 0)
            {
                return NotFound($"No orders found for customer with ID {customerId}");
            }

            var jsonSettings = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };
            var jsonResult = JsonSerializer.Serialize(customerOrders, jsonSettings); //Returns order with its parts

            return Ok(jsonResult);
        }

        [HttpGet("getordersforday")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersForDay(DateTime orderDate)
        {
            // Find all orders for the provided day
            var ordersForDay = await _context.Orders
                .Where(o => o.OrderDate.Date == orderDate.Date)
                .ToListAsync();

            if (ordersForDay == null || ordersForDay.Count == 0)
            {
                return NotFound($"No orders found for the provided day: {orderDate.ToShortDateString()}");
            }

            return Ok(ordersForDay);
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
                OrderDate = orderDto.OrderDate
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
