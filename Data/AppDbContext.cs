using TraineeManagement.Api.Models;
namespace TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
 
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
 
    public DbSet<Trainee> Trainees { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<LearningTask> LearningTasks { get; set; }
    public DbSet<TaskAssignment> TaskAssignment { get; set; }

     protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskAssignment>(entity =>
            {
                  entity.HasOne<Trainee>(ta => ta.Trainee)
                .WithMany(t => t.TaskAssignments)
                .HasForeignKey(ta => ta.TraineeId)
                .IsRequired();

                entity.HasOne<Mentor>(ta => ta.Mentor)
                .WithMany(m => m.TaskAssignments)
                .HasForeignKey(ta => ta.MentorId)
                .IsRequired(); 

                entity.HasOne<LearningTask>(ta => ta.LearningTask)
                .WithMany(t => t.TaskAssignments)
                .HasForeignKey(t => t.LearningTaskId)
                .IsRequired();  
            });
               
        }



}