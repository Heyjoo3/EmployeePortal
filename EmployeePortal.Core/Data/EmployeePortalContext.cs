using Microsoft.EntityFrameworkCore;
using EmployeePortal.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeePortal.Core.Data
{
    public class EmployeePortalContext: DbContext
    {
        public EmployeePortalContext(DbContextOptions options)
           : base(options)
        {
        }
        public DbSet<Employee>? Employees { get; set; }
        public DbSet<Vacation>? Vacations { get; set; }
        public DbSet<PublicHoliday>? PublicHolidays { get; set; }
        public DbSet<Absence>? Absences { get; set; }
        public DbSet<TaskGroup>? TaskGroups { get; set; }
        //public DbSet<Template>? Templates { get; set; }
        public DbSet<BaseTask> Tasks { get; set; }
        public DbSet<OnboardingPlan>? OnboardingPlans { get; set; }


        public async Task<int> SaveChangesWithNoLogAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().ToTable("EmployeeRoles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("EmployeeRoleMappings")
                 .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<Vacation>()
           .Property(e => e.Description)
           .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Absence>()
                .Property(e => e.Remarks)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<OnboardingPlan>()
                .HasOne(tg => tg.ReferenceEmployee)
                .WithMany()
                .HasForeignKey(tg => tg.ReferencePerson)
                .HasPrincipalKey(e => e.Id); 

            modelBuilder.Entity<TaskGroup>()
                .HasOne(tg => tg.ReferenceEmployee)
                .WithMany()
                .HasForeignKey(tg => tg.ReferencePerson)
                .HasPrincipalKey(e => e.Id); 


            modelBuilder.Entity<BaseTask>()
                 .HasOne(b => b.ReferenceEmployee)
                 .WithMany()
                 .HasForeignKey(b => b.ReferencePerson)
                 .OnDelete(DeleteBehavior.Restrict); // Optional: Prevent cascading deletes

            modelBuilder.Entity<BaseTask>()
                .HasDiscriminator<string>("TaskType")
                .HasValue<TodoTask>("TodoTask")
                .HasValue<NetworkingTask>("NetworkingTask")
                .HasValue<ProjectTask>("ProjectTask")
                .HasValue<TimeScheduledTask>("TimeScheduledTask");

            modelBuilder.Entity<TodoTask>()
                .Property(t => t.Description)
                .HasColumnName("Description");
            modelBuilder.Entity<NetworkingTask>()
                .Property(t => t.StartDate)
                .HasColumnName("StartDate");
            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.EndDate)
                .HasColumnName("EndDate");
            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.StartDate)
                .HasColumnName("StartDate");
            modelBuilder.Entity<ProjectTask>()
                .Property(t => t.Description)
                .HasColumnName("Description");
            modelBuilder.Entity<TimeScheduledTask>()
               .Property(t => t.EndDate)
               .HasColumnName("EndDate");
            modelBuilder.Entity<TimeScheduledTask>()
                .Property(t => t.StartDate)
                .HasColumnName("StartDate");
            modelBuilder.Entity<TimeScheduledTask>()
                .Property(t => t.Description)
                .HasColumnName("Description");


        }
    }
}