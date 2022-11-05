using Microsoft.EntityFrameworkCore;
using Project_API.Models.Entities;

namespace Project_API.Data
{

    public class ItemdbContext : DbContext
    {
        public ItemdbContext(DbContextOptions<ItemdbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BorrowedItem>().HasKey(table => new {
                table.ItemId,
                table.StudentId
            });
        }

        public DbSet<Item> Items { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<BorrowedItem> BorrowedItems { get; set; }
        
    }
}
