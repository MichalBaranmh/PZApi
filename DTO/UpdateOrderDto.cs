using PZApi.Models;

namespace PZApi.DTO
{
    public class UpdateOrderDto
    {
        public int? PartId {  get; set; }
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
    }
}
