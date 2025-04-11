using DataBalk.Assessment.Data.Models;
using Microsoft.EntityFrameworkCore;
using Task = DataBalk.Assessment.Data.Models.Task;

namespace DataBalk.Assessment.Data
{
    public class DataBalkAssessmentContext(DbContextOptions<DataBalkAssessmentContext> options ) : DbContext(options)
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tasks)
                .WithOne(t => t.Assignee)
                .HasForeignKey(t => t.AssigneeId);

            modelBuilder.Entity<Task>();
        }

    }
}


