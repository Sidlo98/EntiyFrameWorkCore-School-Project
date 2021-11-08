using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class OrderLineCreateModel
    {
        [Required]
        [ForeignKey("ProductEntity")]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        public int Quantity { get; set; }
    }
}
