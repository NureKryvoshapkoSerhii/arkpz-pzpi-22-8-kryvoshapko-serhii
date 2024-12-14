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
    public class WaterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WaterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Додати запис про водоспоживання
        [HttpPost("add-water/{userId}")]
        public async Task<IActionResult> AddWater(string userId, [FromBody] AddWaterRequest request)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Створення нового запису про водоспоживання
            var waterIntake = new WaterIntake
            {
                user_uid = userId,
                amount_ml = request.amount_ml,
                entry_date = request.entry_date
            };

            _context.WaterIntakes.Add(waterIntake);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Water intake added successfully." });
        }

        // Оновити запис про водоспоживання
        [HttpPut("update-water/{userId}/{intakeId}")]
        public async Task<IActionResult> UpdateWater(string userId, int intakeId, [FromBody] UpdateWaterRequest request)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Перевірка, чи існує запис водоспоживання
            var waterIntake = await _context.WaterIntakes.FindAsync(intakeId);
            if (waterIntake == null || waterIntake.user_uid != userId)
            {
                return NotFound(new { message = "Water intake entry not found." });
            }

            // Оновлення значення води
            waterIntake.amount_ml = request.amount_ml;
            waterIntake.entry_date = request.entry_date;

            _context.Entry(waterIntake).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Water intake updated successfully." });
        }

        // Видалити запис про водоспоживання
        [HttpDelete("delete-water/{userId}/{intakeId}")]
        public async Task<IActionResult> DeleteWater(string userId, int intakeId)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Перевірка, чи існує запис водоспоживання
            var waterIntake = await _context.WaterIntakes.FindAsync(intakeId);
            if (waterIntake == null || waterIntake.user_uid != userId)
            {
                return NotFound(new { message = "Water intake entry not found." });
            }

            _context.WaterIntakes.Remove(waterIntake);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Water intake entry deleted successfully." });
        }

        [HttpGet("get-water/{userId}")]
        public async Task<IActionResult> GetWater(string userId)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Отримуємо тільки записи водоспоживання для користувача
            var waterIntakes = await _context.WaterIntakes
                .Where(w => w.user_uid == userId)
                .Select(w => new
                {
                    w.amount_ml,  // Кількість води в мілілітрах
                    w.entry_date  // Дата запису водоспоживання
                })
                .ToListAsync();

            return Ok(waterIntakes);
        }

    }

    // DTO для додавання води
    public class AddWaterRequest
    {
        public double amount_ml { get; set; } // Кількість води в мілілітрах
        public DateTime entry_date { get; set; } // Дата запису
    }

    // DTO для оновлення води
    public class UpdateWaterRequest
    {
        public double amount_ml { get; set; } // Кількість води в мілілітрах
        public DateTime entry_date { get; set; } // Дата запису
    }
}
