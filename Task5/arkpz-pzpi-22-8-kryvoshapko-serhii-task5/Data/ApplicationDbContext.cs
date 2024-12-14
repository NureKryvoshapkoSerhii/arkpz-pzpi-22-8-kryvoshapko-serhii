using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // DbSets for each model
        public DbSet<User> Users { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<WeightMeasurement> WeightMeasurements { get; set; }
        public DbSet<UserGoal> UserGoals { get; set; }
        public DbSet<MealEntry> MealEntries { get; set; }
        public DbSet<ExerciseEntry> ExerciseEntries { get; set; }
        public DbSet<WaterIntake> WaterIntakes { get; set; }
        public DbSet<StreakHistory> StreakHistories { get; set; }
        public DbSet<UserConsultant> UserConsultants { get; set; }
        public DbSet<ConsultantNote> ConsultantNotes { get; set; }
        public DbSet<ConsultantRequest> ConsultantRequests { get; set; }
        public DbSet<Admin> Admins { get; set; }  // DbSet для Admin

        // Configuring the model relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of configuring one-to-many relationship between User and WeightMeasurement
            modelBuilder.Entity<WeightMeasurement>()
                .HasOne(w => w.User)
                .WithMany(u => u.WeightMeasurements)
                .HasForeignKey(w => w.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGoal>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGoals)
                .HasForeignKey(ug => ug.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGoal>(entity =>
            {
                entity.HasOne(ug => ug.User)
                    .WithMany(u => u.UserGoals)
                    .HasForeignKey(ug => ug.user_uid)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ug => ug.Consultant)
                    .WithMany(c => c.UserGoals)
                    .HasForeignKey(ug => ug.consultant_uid)
                    .IsRequired(false)  // Явно вказуємо що це необов'язкове поле
                    .OnDelete(DeleteBehavior.SetNull);

                // Конфігурація для enum GoalType
                entity.Property(e => e.goal_type)
                    .HasConversion<string>()  // Конвертуємо enum в string
                    .IsRequired()
                    .HasMaxLength(10);  // Обмежуємо довжину як в БД
            });

            modelBuilder.Entity<MealEntry>()
                .HasOne(me => me.User)
                .WithMany(u => u.MealEntries)
                .HasForeignKey(me => me.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExerciseEntry>()
                .HasOne(e => e.User)
                .WithMany(u => u.ExerciseEntries)
                .HasForeignKey(e => e.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WaterIntake>()
                .HasOne(w => w.User)
                .WithMany(u => u.WaterIntakes)
                .HasForeignKey(w => w.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StreakHistory>()
                .HasOne(s => s.User)
                .WithMany(u => u.StreakHistories)
                .HasForeignKey(s => s.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConsultant>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserConsultants)
                .HasForeignKey(uc => uc.user_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserConsultant>()
                .HasOne(uc => uc.Consultant)
                .WithMany(c => c.UserConsultants)
                .HasForeignKey(uc => uc.consultant_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsultantNote>()
                .HasOne(cn => cn.Consultant)
                .WithMany(c => c.ConsultantNotes)
                .HasForeignKey(cn => cn.consultant_uid)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConsultantNote>()
                .HasOne(cn => cn.UserGoal)
                .WithMany(ug => ug.ConsultantNotes)
                .HasForeignKey(cn => cn.goal_id)
                .OnDelete(DeleteBehavior.SetNull);

            // Configuration for Admin table
            modelBuilder.Entity<Admin>()
                .HasKey(a => a.admin_uid);  // Set admin_uid as the primary key

            modelBuilder.Entity<Admin>()
                .Property(a => a.email)
                .IsRequired()  // Make sure email is required
                .HasMaxLength(100);  // Set email length limit

            modelBuilder.Entity<Admin>()
                .Property(a => a.phone_number)
                .IsRequired()  // Make sure phone number is required
                .HasMaxLength(20);  // Set phone number length limit
        }
    }
}
