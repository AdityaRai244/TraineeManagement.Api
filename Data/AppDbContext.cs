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
    public DbSet<Submission> Submission { get; set; }
    public DbSet<Review> Reviews { get; set; }


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

            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasOne<TaskAssignment>(ta => ta.TaskAssignment)
                .WithMany(t => t.Submissions)
                .HasForeignKey(fk => fk.TaskAssignmentId)
                .IsRequired();

            });

             modelBuilder.Entity<Review>(entity =>
            {
                entity.HasOne<Submission>(ta => ta.Submissions)
                .WithMany(t => t.Reviews)
                .HasForeignKey(fk => fk.SubmissionId)
                .IsRequired();

                entity.HasOne<Mentor>()
                .WithMany(t => t.Reviews)
                .HasForeignKey(fk => fk.MentorId)
                .IsRequired();

            });

            



         

             
               
        }



}