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
    public class StreakController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StreakController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Додати новий запис стрика
        [HttpPost("add-streak/{userId}")]
        public async Task<IActionResult> AddStreak(string userId, [FromBody] AddStreakRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Створення нового запису для стрика
            var streakHistory = new StreakHistory
            {
                user_uid = userId,
                streak_date = DateTime.UtcNow,
                current_streak = 1, // Стартовий стрик
                is_active = true
            };

            _context.StreakHistories.Add(streakHistory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "New streak started successfully." });
        }

        [HttpPut("update-streak/{userId}")]
        public async Task<IActionResult> UpdateStreak(string userId, [FromBody] UpdateStreakRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var streakHistory = await _context.StreakHistories
                .Where(s => s.user_uid == userId && s.is_active)
                .OrderByDescending(s => s.streak_date)
                .FirstOrDefaultAsync();

            if (streakHistory == null)
            {
                return NotFound(new { message = "No active streak found." });
            }

            // Оновлення поточного стрика
            streakHistory.current_streak = request.current_streak;
            streakHistory.streak_date = DateTime.UtcNow; // Оновлюємо дату стрика
            streakHistory.is_active = request.is_active; // Оновлення активності

            _context.Entry(streakHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Streak updated successfully." });
        }

        // Отримати історію стриків для користувача
        [HttpGet("get-streaks/{userId}")]
        public async Task<IActionResult> GetStreaks(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Вибираємо всі стрики користувача, незалежно від їхнього статусу (активний чи ні)
            var streakHistories = await _context.StreakHistories
                .Where(s => s.user_uid == userId)  // Не фільтруємо за активністю
                .OrderByDescending(s => s.streak_date)  // Сортуємо за датою, новіші перші
                .Select(s => new
                {
                    s.streak_id,
                    s.streak_date,
                    s.current_streak,
                    s.is_active  // Виводимо статус активності
                })
                .ToListAsync();

            return Ok(streakHistories);
        }



        // Видалити стрик (можливо, для скидання)
        [HttpDelete("delete-streak/{userId}")]
        public async Task<IActionResult> DeleteStreak(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var streakHistory = await _context.StreakHistories
                .Where(s => s.user_uid == userId && s.is_active)
                .OrderByDescending(s => s.streak_date)
                .FirstOrDefaultAsync();

            if (streakHistory == null)
            {
                return NotFound(new { message = "No active streak found." });
            }

            // Видалення стрика
            streakHistory.is_active = false; // Можна просто позначити як неактивний, замість повного видалення
            _context.Entry(streakHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Streak deleted successfully." });
        }
    }

    // DTO для додавання нового стрика
    public class AddStreakRequest
    {
        public int current_streak { get; set; }
    }

    // DTO для оновлення стрика
    public class UpdateStreakRequest
    {
        public int current_streak { get; set; }
        public bool is_active { get; set; }
    }
}
