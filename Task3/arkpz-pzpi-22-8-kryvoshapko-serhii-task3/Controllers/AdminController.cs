using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NutriTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("remove-user/{userUid}")]
        public async Task<IActionResult> RemoveUser(string userUid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete all weight measurements
                await _context.WeightMeasurements
                    .Where(w => w.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete all meal entries
                await _context.MealEntries
                    .Where(m => m.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete all exercise entries
                await _context.ExerciseEntries
                    .Where(e => e.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete all water intakes
                await _context.WaterIntakes
                    .Where(w => w.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete all streak histories
                await _context.StreakHistories
                    .Where(s => s.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete consultant notes for this user
                await _context.ConsultantNotes
                    .Where(cn => cn.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete consultant requests
                await _context.ConsultantRequests
                    .Where(cr => cr.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete user-consultant relationships
                await _context.UserConsultants
                    .Where(uc => uc.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Delete user goals
                await _context.UserGoals
                    .Where(ug => ug.user_uid == userUid)
                    .ExecuteDeleteAsync();

                // Finally, delete the user
                await _context.Users
                    .Where(u => u.user_uid == userUid)
                    .ExecuteDeleteAsync();

                await transaction.CommitAsync();
                return Ok(new { message = "User and all related data removed successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error removing user data.", error = ex.Message });
            }
        }

        [HttpDelete("remove-consultant/{consultantUid}")]
        public async Task<IActionResult> RemoveConsultant(string consultantUid)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Delete consultant notes
                await _context.ConsultantNotes
                    .Where(cn => cn.consultant_uid == consultantUid)
                    .ExecuteDeleteAsync();

                // Delete consultant requests
                await _context.ConsultantRequests
                    .Where(cr => cr.consultant_uid == consultantUid)
                    .ExecuteDeleteAsync();

                // Delete user-consultant relationships
                await _context.UserConsultants
                    .Where(uc => uc.consultant_uid == consultantUid)
                    .ExecuteDeleteAsync();

                // Update user goals to remove consultant reference
                await _context.UserGoals
                    .Where(ug => ug.consultant_uid == consultantUid)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(b => b.consultant_uid, (string)null)
                        .SetProperty(b => b.is_approved_by_consultant, false));

                // Finally, delete the consultant
                await _context.Consultants
                    .Where(c => c.consultant_uid == consultantUid)
                    .ExecuteDeleteAsync();

                await transaction.CommitAsync();
                return Ok(new { message = "Consultant and all related data removed successfully." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error removing consultant data.", error = ex.Message });
            }
        }


        [HttpGet("get-user-info")]
        public async Task<IActionResult> GetUserInfo([FromQuery] string nickname, [FromQuery] DateTime createdAt, [FromQuery] DateTime lastLogin)
        {
            var user = await _context.Users
                .Where(u => u.nickname == nickname && u.created_at == createdAt && u.last_login == lastLogin)
                .Select(u => new
                {
                    u.user_uid,
                    u.nickname,
                    u.created_at,
                    u.last_login,
                    u.gender,
                    u.height,
                    u.current_weight
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        [HttpGet("get-consultant-info")]
        public async Task<IActionResult> GetConsultantInfo([FromQuery] string nickname, [FromQuery] DateTime createdAt, [FromQuery] DateTime lastLogin)
        {
            var consultant = await _context.Consultants
                .Where(c => c.nickname == nickname && c.created_at == createdAt && c.last_login == lastLogin)
                .Select(c => new
                {
                    c.consultant_uid,
                    c.nickname,
                    c.created_at,
                    c.last_login,
                    c.current_clients,
                    c.max_clients,
                    c.experience_years
                })
                .FirstOrDefaultAsync();

            if (consultant == null)
            {
                return NotFound(new { message = "Consultant not found." });
            }

            return Ok(consultant);
        }

        // 3. Статистика додатку (кількість зареєстрованих користувачів та консультантів, активних тощо)
        [HttpGet("get-statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var userCount = await _context.Users.CountAsync();
            var activeUserCount = await _context.Users.CountAsync(u => u.is_active);

            var consultantCount = await _context.Consultants.CountAsync();
            var activeConsultantCount = await _context.Consultants.CountAsync(c => c.is_active);

            var statistics = new
            {
                TotalUsers = userCount,
                ActiveUsers = activeUserCount,
                TotalConsultants = consultantCount,
                ActiveConsultants = activeConsultantCount
            };

            return Ok(statistics);
        }

        [HttpPatch("update-user-profile/{user_uid}")]
        public async Task<IActionResult> UpdateUserProfile(string user_uid, [FromBody] UpdateProfileDto updateDto)
        {
            var user = await _context.Users.FindAsync(user_uid);
            if (user == null)
            {
                return NotFound(new { message = "Користувача не знайдено" });
            }

            // Оновлюємо тільки надані поля
            if (updateDto.Nickname != null)
            {
                user.nickname = updateDto.Nickname;
            }
            if (updateDto.ProfilePicture != null)
            {
                user.profile_picture = updateDto.ProfilePicture;
            }
            if (updateDto.ProfileDescription != null)
            {
                user.profile_description = updateDto.ProfileDescription;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Профіль користувача успішно оновлено" });
        }

        [HttpPatch("update-consultant-profile/{consultant_uid}")]
        public async Task<IActionResult> UpdateConsultantProfile(string consultant_uid, [FromBody] UpdateConsultantProfileDto updateDto)
        {
            var consultant = await _context.Consultants.FindAsync(consultant_uid);
            if (consultant == null)
            {
                return NotFound(new { message = "Консультанта не знайдено" });
            }

            // Оновлюємо тільки надані поля
            if (updateDto.Nickname != null)
            {
                consultant.nickname = updateDto.Nickname;
            }
            if (updateDto.ProfilePicture != null)
            {
                consultant.profile_picture = updateDto.ProfilePicture;
            }
            if (updateDto.ProfileDescription != null)
            {
                consultant.profile_description = updateDto.ProfileDescription;
            }
            if (updateDto.ExperienceYears != null)
            {
                consultant.experience_years = updateDto.ExperienceYears.Value;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Профіль консультанта успішно оновлено" });
        }
    }
}
    public class UpdateProfileDto
    {
        public string? Nickname { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProfileDescription { get; set; }
    }

    public class UpdateConsultantProfileDto : UpdateProfileDto
    {
        public int? ExperienceYears { get; set; }
    }


