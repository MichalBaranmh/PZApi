using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZApi.DTO;
using PZApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PZApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CarServiceConext _context;
        public CustomerController(CarServiceConext context)
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
    }
}
