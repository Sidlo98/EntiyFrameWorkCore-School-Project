using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Table("UserHashes")]
    public class UserHashEntity
    {
        [Key]
        [Required]
        [ForeignKey("UserEntity")]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(max)")]
        public string UserHash { get; set; }


        // Relations
        public virtual UserEntity User { get; set; }
    }
}
