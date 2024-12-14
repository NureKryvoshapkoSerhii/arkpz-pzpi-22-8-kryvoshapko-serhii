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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Оновлення нікнейму користувача
        [HttpPut("update-nickname/{userId}")]
        public async Task<IActionResult> UpdateNickname(string userId, [FromBody] UpdateNicknameRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.nickname = request.new_nickname;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Nickname updated successfully." });
        }

        // Оновлення фото профілю користувача
        [HttpPut("update-profile-picture/{userId}")]
        public async Task<IActionResult> UpdateProfilePicture(string userId, [FromBody] UpdateProfilePictureRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.profile_picture = request.new_profile_picture;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile picture updated successfully." });
        }

        // Оновлення опису профілю користувача
        [HttpPut("update-profile-description/{userId}")]
        public async Task<IActionResult> UpdateProfileDescription(string userId, [FromBody] UpdateProfileDescriptionRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.profile_description = request.new_profile_description;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile description updated successfully." });
        }

        // Оновлення ваги користувача (current_weight)
        [HttpPut("update-current-weight/{userId}")]
        public async Task<IActionResult> UpdateCurrentWeight(string userId, [FromBody] UpdateCurrentWeightRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Оновлюємо вагу користувача
            user.current_weight = request.new_current_weight;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "User current weight updated successfully." });
        }

        // Видалити консультанта з користувача
        [HttpDelete("remove-consultant/{userId}")]
        public async Task<IActionResult> RemoveConsultant(string userId, [FromBody] RemoveConsultantRequest request)
        {
            var userConsultant = await _context.UserConsultants
                .FirstOrDefaultAsync(uc => uc.user_uid == userId && uc.consultant_uid == request.consultant_uid);

            if (userConsultant == null)
            {
                return NotFound(new { message = "Consultant not found or not assigned to this user." });
            }

            // Видалення зв'язку між консультантом і користувачем
            _context.UserConsultants.Remove(userConsultant);
            await _context.SaveChangesAsync();

            // Оновлення кількості клієнтів консультанта
            var consultant = await _context.Consultants.FindAsync(request.consultant_uid);
            if (consultant != null)
            {
                consultant.current_clients -= 1; // Зменшуємо кількість клієнтів для консультанта
                _context.Entry(consultant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            // Видалити всі активні запити цього користувача
            var consultantRequests = await _context.ConsultantRequests
                .Where(cr => cr.user_uid == userId && cr.consultant_uid == request.consultant_uid && cr.status == "accepted")
                .ToListAsync();

            if (consultantRequests.Any())
            {
                _context.ConsultantRequests.RemoveRange(consultantRequests);  // Видаляємо активні запити
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Consultant removed successfully and pending requests deleted." });
        }
    }

    // DTOs
    public class UpdateNicknameRequest
    {
        public string new_nickname { get; set; }
    }

    public class UpdateProfilePictureRequest
    {
        public string new_profile_picture { get; set; }
    }

    public class UpdateProfileDescriptionRequest
    {
        public string new_profile_description { get; set; }
    }

    public class UpdateCurrentWeightRequest
    {
        public double new_current_weight { get; set; }
    }

    public class RemoveConsultantRequest
    {
        public string consultant_uid { get; set; }
    }
}
