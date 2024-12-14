using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutriTrack.Models
{

    public class Admin
    {
        [Key]
        public string admin_uid { get; set; }  // Унікальний ідентифікатор адміністратора

        [Required]
        public DateTime registration_date { get; set; }  // Дата реєстрації адміністратора

        [Required]
        [MaxLength(100)]
        public string name { get; set; }  // Ім'я адміністратора

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string email { get; set; }  // Електронна пошта адміністратора

        [Required]
        [RegularExpression(@"^\+380\d{9}$", ErrorMessage = "Phone number must be in the format +380XXXXXXXXX")]
        [MaxLength(20)]
        public string phone_number { get; set; }  // Номер телефону в форматі +380
    }

    public class User
    {
        [Key]
        public string user_uid { get; set; }

        [Required]
        [MaxLength(50)]
        public string nickname { get; set; }

        public string profile_picture { get; set; }
        public string profile_description { get; set; }
        public string gender { get; set; }
        public int? height { get; set; }
        public double? current_weight { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow; // Значення за замовчуванням
        public DateTime? last_login { get; set; }
        public bool is_active { get; set; }

        // Ініціалізація колекцій в конструкторі
        public User()
        {
            WeightMeasurements = new HashSet<WeightMeasurement>();
            UserGoals = new HashSet<UserGoal>();
            MealEntries = new HashSet<MealEntry>();
            ExerciseEntries = new HashSet<ExerciseEntry>();
            WaterIntakes = new HashSet<WaterIntake>();
            StreakHistories = new HashSet<StreakHistory>();
            UserConsultants = new HashSet<UserConsultant>();
            ConsultantNotes = new HashSet<ConsultantNote>();
            ConsultantRequests = new HashSet<ConsultantRequest>();
        }

        // Навігаційні властивості
        public virtual ICollection<WeightMeasurement> WeightMeasurements { get; set; }
        public virtual ICollection<UserGoal> UserGoals { get; set; }
        public virtual ICollection<MealEntry> MealEntries { get; set; }
        public virtual ICollection<ExerciseEntry> ExerciseEntries { get; set; }
        public virtual ICollection<WaterIntake> WaterIntakes { get; set; }
        public virtual ICollection<StreakHistory> StreakHistories { get; set; }
        public virtual ICollection<UserConsultant> UserConsultants { get; set; }
        public virtual ICollection<ConsultantNote> ConsultantNotes { get; set; }
        public virtual ICollection<ConsultantRequest> ConsultantRequests { get; set; }
    }

    public class Consultant
    {
        [Key]
        public string consultant_uid { get; set; }

        [Required]

        [MaxLength(50)]
        public string nickname { get; set; }

        public string profile_picture { get; set; }

        // Додаємо нове поле для опису профілю
        public string profile_description { get; set; }

        [Required]
        public int experience_years { get; set; }

        public bool is_active { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime? last_login { get; set; }

        [Required]
        public int max_clients { get; set; }

        public int current_clients { get; set; }

        public Consultant()
        {
            UserGoals = new HashSet<UserGoal>();
            UserConsultants = new HashSet<UserConsultant>();
            ConsultantNotes = new HashSet<ConsultantNote>();
            ConsultantRequests = new HashSet<ConsultantRequest>();
        }

        public virtual ICollection<UserGoal> UserGoals { get; set; }
        public virtual ICollection<UserConsultant> UserConsultants { get; set; }
        public virtual ICollection<ConsultantNote> ConsultantNotes { get; set; }
        public virtual ICollection<ConsultantRequest> ConsultantRequests { get; set; }
    }


    public class WeightMeasurement
    {
        [Key]
        public int measurement_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        public double weight { get; set; }

        public DateTime measured_at { get; set; } = DateTime.UtcNow; // Значення за замовчуванням
        public string device_id { get; set; }
        public bool is_synced { get; set; }

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
    }

    public enum GoalType
    {
        Gain,
        Loss,
        Maintain
    }

    public class UserGoal
    {
        [Key]
        public int goal_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        public string? consultant_uid { get; set; }  // Зробили nullable

        [Required]
        [Column(TypeName = "nvarchar(10)")]  // Вказуємо точний тип як у БД
        public GoalType goal_type { get; set; }

        [Required]
        public double target_weight { get; set; }

        [Required]
        public int duration_weeks { get; set; }

        [Required]
        public double daily_calories { get; set; }

        [Required]
        public double daily_protein { get; set; }

        [Required]
        public double daily_carbs { get; set; }

        [Required]
        public  double daily_fats { get; set; }

        [Required]
        [Column(TypeName = "date")]  // Вказуємо точний тип як у БД
        public DateTime start_date { get; set; }

        [Required]
        public bool is_approved_by_consultant { get; set; }

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }

        [ForeignKey("consultant_uid")]
        public virtual Consultant? Consultant { get; set; }  // Зробили nullable

        public virtual ICollection<ConsultantNote>? ConsultantNotes { get; set; }
    }

    public class MealEntry
    {
        [Key]
        public int entry_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        [MaxLength(10)]
        public string meal_type { get; set; } // breakfast, lunch, dinner, snack

        [Required]
        public DateTime entry_date { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(100)]
        public string product_name { get; set; } // Назва продукту

        [Required]
        public double quantity_grams { get; set; }

        [Required]
        public double calories { get; set; }

        [Required]
        public double protein { get; set; }

        [Required]
        public double carbs { get; set; }

        [Required]
        public double fats { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
    }



    public class ExerciseEntry
    {
        [Key]
        public int exercise_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        [MaxLength(100)]
        public string exercise_name { get; set; }

        [Required]
        public int duration_minutes { get; set; }

        [Required]
        public double calories_burned { get; set; }

        public string exercise_type { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateTime.UtcNow; // Значення за замовчуванням

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
    }

    [Table("WaterIntake")]
    public class WaterIntake
    {

        [Key]
        public int intake_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        public double amount_ml { get; set; }

        [Required]
        public DateTime entry_date { get; set; } = DateTime.UtcNow; // Значення за замовчуванням

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
    }

    [Table("StreakHistory")]
    public class StreakHistory
    {
        [Key]
        public int streak_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        public DateTime streak_date { get; set; } = DateTime.UtcNow; // Значення за замовчуванням

        [Required]
        public int current_streak { get; set; }

        public bool is_active { get; set; }

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
    }

    public class UserConsultant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string user_consultant_id { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        public string consultant_uid { get; set; }

        public DateTime assignment_date { get; set; } = DateTime.UtcNow; // Значення за замовчуванням

        public bool is_active { get; set; }

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }

        [ForeignKey("consultant_uid")]
        public virtual Consultant Consultant { get; set; }
    }

    public class ConsultantNote
    {
        [Key]
        public int note_id { get; set; }

        [Required]
        public string consultant_uid { get; set; }

        [Required]
        public int goal_id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string content { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        public string user_uid { get; set; }
        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
        [ForeignKey("consultant_uid")]
        public virtual Consultant Consultant { get; set; }

        [ForeignKey("goal_id")]
        public virtual UserGoal UserGoal { get; set; } // Можемо отримати користувача через зв’язок із UserGoal
    }


    public class ConsultantRequest
    {
        [Key]
        public int request_id { get; set; }

        [Required]
        public string consultant_uid { get; set; }

        [Required]
        public string user_uid { get; set; }

        [Required]
        public string status { get; set; } // Наприклад, "pending", "accepted", "rejected"

        public DateTime created_at { get; set; } = DateTime.UtcNow; // Значення за замовчуванням

        public DateTime responded_at { get; set; } = DateTime.UtcNow; // Значення за замовчуванням


        [ForeignKey("consultant_uid")]
        public virtual Consultant Consultant { get; set; }

        [ForeignKey("user_uid")]
        public virtual User User { get; set; }
    }
}
