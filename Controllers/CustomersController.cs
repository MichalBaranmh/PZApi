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
    public class CustomersController : ControllerBase
    {
        private readonly CarServiceConext _context;
        public CustomersController(CarServiceConext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers() 
        {
            var customers = await _context.Customers.ToListAsync();
            if (customers == null || !customers.Any()) 
            {
                return NotFound();
            }

            return Ok(customers);

        }
        //Get Customer by CustomerID
        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Customer not found");
            }

            return Ok(customer);
        }

        [HttpGet("getorders/{customerId}")]
        public async Task<ActionResult<IEnumerable<Part>>> GetOrdersByCustomerId(int customerId)
        {

            var orders = await _context.Orders
                .Where(order => order.CustomerId == customerId)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found");
            }

            var jsonSettings = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };
            var jsonResult = JsonSerializer.Serialize(orders, jsonSettings); //Returns order with its parts

            return Ok(jsonResult);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCustomer([FromBody] CustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newCustomer = new Customer
            {
                CustomerName = customerDto.CustomerName,
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
