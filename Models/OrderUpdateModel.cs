using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class OrderUpdateModel
    {
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; }

        public virtual ICollection<OrderLineCreateModel> OrderLines { get; set; }
    }
}
