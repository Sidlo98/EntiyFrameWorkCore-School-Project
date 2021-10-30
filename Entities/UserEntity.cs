using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }


        // Relations
        public virtual ProfileEntity Profile { get;set; }
        public virtual UserHashEntity UserHash { get; set; }
        public virtual ICollection<OrderEntity> Orders { get; set; }
    }
}
