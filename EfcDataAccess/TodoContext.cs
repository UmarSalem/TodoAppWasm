using Shared.Models;
using Microsoft.EntityFrameworkCore;
namespace EfcDataAccess
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Todo>()
                .Property(todo => todo.OwnerId)
                .HasColumnName("UserId");

            modelBuilder.Entity<Todo>()
                .HasOne(todo => todo.Owner)
                .WithMany()
                .HasForeignKey(todo => todo.OwnerId);
        }

    }
}
