using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("OrderLines")]
    public class OrderLineEntity
    {   
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("OrderEntity")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("ProductEntity")]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "tinyint")]
        public int Quantity { get; set; }


        //Relations
        public virtual OrderEntity Order { get; set; }
        public virtual ProductEntity Product { get; set; }
    }
}
