using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PZApi.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ServiceName { get; set; }
        public decimal? ServicePrice { get; set; }

        //Customer musi istniec do stworzenie orderu
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public ICollection<Part> Parts { get; set; }
    }
}
