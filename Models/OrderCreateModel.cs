using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class OrderCreateModel
    {
        [Required]
        [ForeignKey("UserEntity")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        public virtual ICollection<OrderLineCreateModel> OrderLines { get; set; }
    }
}
