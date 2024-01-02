using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PZApi.DTO;
using PZApi.Models;

namespace PZApi.Controllers
{
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly CarServiceConext _context;

        public PartsController(CarServiceConext context)
        {
            _context = context;
        }

        // GET: api/Parts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Part>>> GetAllParts()
        {
            var parts = await _context.Parts.ToListAsync();

            if (parts == null || !parts.Any())
            {
                return NotFound();
            }

            return Ok(parts);
        }
        // GET: api/Parts
        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> GetPartById(int id)
        {
            var part = await _context.Parts.FindAsync(id);

            if (part == null)
            {
                return NotFound();
            }

            return Ok(part);
        }

        public async Task<ActionResult<IEnumerable<Part>>> GetOrdersByOrderId(int orderId)
        {
            var orderParts = _context.Parts
                .Where(part => part.OrderID == orderId)
                .ToList();

            if (orderParts == null || !orderParts.Any())
            {
                return NotFound();
            }

            return Ok(orderParts);
        }
        // POST: api/Parts/{id}
        [HttpPost]
        public async Task<ActionResult> CreatePart([FromBody] PartDto partDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newPart = new Part
            {
                partName = partDto.partName,
                partPrice = partDto.partPrice
            };

                _context.Parts.Add(newPart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPartById), new { id = newPart.PartId }, newPart);
        }
        // DELETE: api/Parts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // PATCH: api/Parts/updateorder
        [HttpPatch("updateorder")]
        public async Task<ActionResult> UpdateOrderForPart([FromBody] UpdateOrderDto updateOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPart = await _context.Parts.FindAsync(updateOrderDto.PartId);

            if (existingPart == null)
            {
                return NotFound(); // Return 404 if the part with the specified ID is not found
            }

            // Update OrderID and Order if provided in the UpdateOrderDto
            if (updateOrderDto.OrderId.HasValue)
            {
                var order = await _context.Orders.FindAsync(updateOrderDto.OrderId.Value);

                if (order != null)
                {
                    existingPart.OrderID = updateOrderDto.OrderId.Value;
                    existingPart.Order = order;

                    await _context.SaveChangesAsync();
                    var jsonSettings = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.Preserve,
                    };
                    var jsonResult = JsonSerializer.Serialize(existingPart, jsonSettings); //Returns updated part with order details

                    return Ok(jsonResult); // Return the updated part
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
