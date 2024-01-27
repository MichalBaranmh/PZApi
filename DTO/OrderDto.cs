namespace PZApi.DTO
{
    public class OrderDto
    {
        public string ServiceName { get; set; }
        public decimal? ServicePrice { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
