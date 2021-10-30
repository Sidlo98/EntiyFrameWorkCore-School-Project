using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("Orders")]
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UserEntity")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }


        // Relations
        public virtual UserEntity User { get; set; }
        public virtual ICollection<OrderLineEntity> OrderLines { get; set; }
    }
}
