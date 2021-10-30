using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi
{
    public class WebApiContext : DbContext
    {
        public  WebApiContext(DbContextOptions<WebApiContext> options) : base(options)
        {
        }

        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<ProfileEntity> Profiles { get; set; }
        public virtual DbSet<UserHashEntity> UserHashes { get; set; }
        public virtual DbSet<OrderEntity> Orders { get; set; }
        public virtual DbSet<OrderLineEntity> OrderLines { get; set; }
        public virtual DbSet<ProductEntity> Products { get; set; }
    }
    
}
