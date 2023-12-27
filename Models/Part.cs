using System.ComponentModel.DataAnnotations;
using PZApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;

namespace PZApi.Models
{
    public class Part
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartId { get; set; }
        public DateTime? shipmentDate { get; set; }
        public string? partName { get; set; }
        public decimal? partPrice { get; set; }


        //Part nie musi nalezec do orderu
        public int? OrderID { get; set; } = null;
        public Order? Order { get; set; }
    }
}
