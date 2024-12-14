using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NutriTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExerciseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExerciseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Додати запис про вправу
        [HttpPost("add-exercise/{userId}")]
        public async Task<IActionResult> AddExercise(string userId, [FromBody] AddExerciseRequest request)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Створення нового запису про вправу
            var exerciseEntry = new ExerciseEntry
            {
                user_uid = userId,
                exercise_name = request.exercise_name,
                duration_minutes = request.duration_minutes,
                calories_burned = request.calories_burned,
                exercise_type = request.exercise_type,
                entry_date = request.entry_date
            };

            _context.ExerciseEntries.Add(exerciseEntry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise entry added successfully." });
        }

        // Оновити запис про вправу
        [HttpPut("update-exercise/{userId}/{exerciseId}")]
        public async Task<IActionResult> UpdateExercise(string userId, int exerciseId, [FromBody] UpdateExerciseRequest request)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Перевірка, чи існує запис вправи
            var exerciseEntry = await _context.ExerciseEntries.FindAsync(exerciseId);
            if (exerciseEntry == null || exerciseEntry.user_uid != userId)
            {
                return NotFound(new { message = "Exercise entry not found." });
            }

            // Оновлення значень
            exerciseEntry.exercise_name = request.exercise_name;
            exerciseEntry.duration_minutes = request.duration_minutes;
            exerciseEntry.calories_burned = request.calories_burned;
            exerciseEntry.exercise_type = request.exercise_type;
            exerciseEntry.entry_date = request.entry_date;

            _context.Entry(exerciseEntry).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise entry updated successfully." });
        }

        // Видалити запис про вправу
        [HttpDelete("delete-exercise/{userId}/{exerciseId}")]
        public async Task<IActionResult> DeleteExercise(string userId, int exerciseId)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Перевірка, чи існує запис вправи
            var exerciseEntry = await _context.ExerciseEntries.FindAsync(exerciseId);
            if (exerciseEntry == null || exerciseEntry.user_uid != userId)
            {
                return NotFound(new { message = "Exercise entry not found." });
            }

            _context.ExerciseEntries.Remove(exerciseEntry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exercise entry deleted successfully." });
        }

        // Отримати записи вправ для користувача
        [HttpGet("get-exercises/{userId}")]
        public async Task<IActionResult> GetExercises(string userId)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Отримуємо всі записи вправ для користувача
            var exerciseEntries = await _context.ExerciseEntries
                .Where(e => e.user_uid == userId)
                .ToListAsync();

            return Ok(exerciseEntries);
        }
    }

    // DTO для додавання вправи
    public class AddExerciseRequest
    {
        public string exercise_name { get; set; } // Назва вправи
        public int duration_minutes { get; set; } // Тривалість вправи в хвилинах
        public float calories_burned { get; set; } // Кількість спалених калорій
        public string exercise_type { get; set; } // Тип вправи (наприклад, кардіо, силові)
        public DateTime entry_date { get; set; } // Дата запису
    }

    // DTO для оновлення вправи
    public class UpdateExerciseRequest
    {
        public string exercise_name { get; set; } // Назва вправи
        public int duration_minutes { get; set; } // Тривалість вправи в хвилинах
        public float calories_burned { get; set; } // Кількість спалених калорій
        public string exercise_type { get; set; } // Тип вправи
        public DateTime entry_date { get; set; } // Дата запису
    }
}
