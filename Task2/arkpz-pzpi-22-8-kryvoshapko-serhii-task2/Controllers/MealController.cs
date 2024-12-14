using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NutriTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MealController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Додати прийом їжі з продуктами
        [HttpPost("add-meal/{userId}")]
        public async Task<IActionResult> AddMeal(string userId, [FromBody] CreateMealRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            foreach (var product in request.products)
            {
                var mealEntry = new MealEntry
                {
                    user_uid = userId,
                    meal_type = request.meal_type,
                    entry_date = DateTime.UtcNow.Date, // Тільки дата
                    product_name = product.product_name,
                    quantity_grams = product.quantity_grams,
                    calories = (product.calories / 100) * product.quantity_grams,
                    protein = (product.protein / 100) * product.quantity_grams,
                    carbs = (product.carbs / 100) * product.quantity_grams,
                    fats = (product.fats / 100) * product.quantity_grams,
                    created_at = DateTime.UtcNow
                };

                _context.MealEntries.Add(mealEntry);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Meal and products added successfully." });
        }

        // Отримати всі продукти для прийому їжі
        [HttpGet("get-meal/{userId}/{entryDate}/{mealType}")]
        public async Task<IActionResult> GetMeal(string userId, DateTime entryDate, string mealType)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var products = await _context.MealEntries
                .Where(me => me.user_uid == userId && me.entry_date.Date == entryDate.Date && me.meal_type == mealType)
                .Select(me => new
                {
                    me.entry_id, // Унікальний ідентифікатор продукту
                    me.product_name,
                    me.quantity_grams,
                    me.calories,
                    me.protein,
                    me.carbs,
                    me.fats,
                    me.created_at
                })
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound(new { message = "No meal entries found for the specified date and type." });
            }

            return Ok(products);
        }


        // Видалити прийом їжі за різними параметрами
        [HttpDelete("delete-meal/{userId}")]
        public async Task<IActionResult> DeleteMeal(string userId, [FromQuery] int? entryId, [FromQuery] DateTime? entryDate, [FromQuery] string mealType)
        {
            // Перевірка, чи існує користувач
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Фільтруємо записи залежно від наданих параметрів
            var query = _context.MealEntries.AsQueryable();

            query = query.Where(me => me.user_uid == userId);

            // Якщо передано entryId, фільтруємо по ньому
            if (entryId.HasValue)
            {
                query = query.Where(me => me.entry_id == entryId.Value);
            }
            // Якщо передано entryDate та mealType, фільтруємо по них
            else if (entryDate.HasValue && !string.IsNullOrEmpty(mealType))
            {
                query = query.Where(me => me.entry_date.Date == entryDate.Value.Date && me.meal_type == mealType);
            }
            else
            {
                return BadRequest(new { message = "Either entryId or both entryDate and mealType must be provided." });
            }

            var mealsToDelete = await query.ToListAsync();

            if (!mealsToDelete.Any())
            {
                return NotFound(new { message = "No meal entries found matching the provided criteria." });
            }

            _context.MealEntries.RemoveRange(mealsToDelete);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Meal entries deleted successfully.", deletedCount = mealsToDelete.Count });
        }


    }

    public class CreateMealRequest
    {
        public string meal_type { get; set; } // Тип прийому їжі (сніданок, обід, вечеря, перекус)
        public List<MealProductRequest> products { get; set; } // Продукти в прийомі їжі
    }

    public class MealProductRequest
    {
        public string product_name { get; set; }
        public double quantity_grams { get; set; }
        public double calories { get; set; }
        public double protein { get; set; }
        public double carbs { get; set; }
        public double fats { get; set; }
    }
}
