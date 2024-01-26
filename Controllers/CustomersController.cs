using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZApi.DTO;
using PZApi.Models;

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

        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
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

        //Update Customer Orders
        [HttpPatch("updateorder")]
        public async Task<ActionResult> UpdateOrderForPart([FromBody] UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCustomer = await _context.Customers.FindAsync(updateOrderDto.CustomerId);

            if (existingCustomer == null)
            {
                return NotFound(); // Return 404 if the customer with specified ID is not found
            }

            // Update customer orders
            if (updateOrderDto.OrderId.HasValue)
            {
                var order = await _context.Orders.FindAsync(updateOrderDto.OrderId.Value);

                if (order != null)
                {
                    existingCustomer.Orders.Add(order);

                    await _context.SaveChangesAsync();

                    return Ok(existingCustomer); // Return the updated part
                }
                else
                {
                    return BadRequest("Invalid OrderID"); // Return 400 if the OrderID is invalid
                }
            }
            else
            {
                return BadRequest("OrderID is required"); // Return 400 if OrderID is not provided
            }
        }
    }
}
