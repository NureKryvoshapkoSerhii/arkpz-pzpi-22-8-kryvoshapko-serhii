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
    public class ConsultantController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConsultantController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Відправити запит на приєднання користувача
        [HttpPost("send-invite/{consultantId}")]
        public async Task<IActionResult> SendInviteToUser(string consultantId, [FromBody] InviteUserRequest request)
        {
            var consultant = await _context.Consultants.FindAsync(consultantId);
            if (consultant == null)
            {
                return NotFound(new { message = "Consultant not found." });
            }

            // Перевірка, чи є вільні місця
            if (consultant.current_clients >= consultant.max_clients)
            {
                return BadRequest(new { message = "No available slots for new clients." });
            }

            var user = await _context.Users.FindAsync(request.user_uid);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Створення запиту на приєднання
            var consultantRequest = new ConsultantRequest
            {
                consultant_uid = consultantId,
                user_uid = request.user_uid,
                status = "pending",  // Статус запиту — "pending"
                created_at = DateTime.UtcNow
            };

            _context.ConsultantRequests.Add(consultantRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invite sent successfully." });
        }

        // Користувач приймає або відхиляє запит
        [HttpPost("respond-invite/{userId}")]
        public async Task<IActionResult> RespondToInvite(string userId, [FromBody] RespondToInviteRequest request)
        {
            var consultantRequest = await _context.ConsultantRequests
                .Where(cr => cr.user_uid == userId && cr.consultant_uid == request.consultant_uid)
                .FirstOrDefaultAsync();

            if (consultantRequest == null)
            {
                return NotFound(new { message = "Invite not found." });
            }

            // Зміна статусу запиту
            if (request.is_accepted)
            {
                consultantRequest.status = "accepted"; // Прийнято

                // Створення зв'язку між користувачем і консультантом
                var userConsultant = new UserConsultant
                {
                    user_uid = userId,
                    consultant_uid = request.consultant_uid,
                    is_active = true,  // Активний зв'язок
                    assignment_date = DateTime.UtcNow
                };

                _context.UserConsultants.Add(userConsultant);

                // Оновлення кількості клієнтів консультанта
                var consultant = await _context.Consultants.FindAsync(request.consultant_uid);
                if (consultant != null)
                {
                    consultant.current_clients += 1;  // Збільшуємо кількість клієнтів
                    _context.Entry(consultant).State = EntityState.Modified;
                }
            }
            else
            {
                consultantRequest.status = "rejected"; // Відхилено
            }

            consultantRequest.responded_at = DateTime.UtcNow;

            _context.Entry(consultantRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invite response recorded." });
        }

        // Метод для видалення користувача консультантом
        [HttpDelete("remove-user/{consultantId}/{userId}")]
        public async Task<IActionResult> RemoveUser(string consultantId, string userId)
        {
            var userConsultant = await _context.UserConsultants
                .FirstOrDefaultAsync(uc => uc.user_uid == userId && uc.consultant_uid == consultantId);

            if (userConsultant == null)
            {
                return NotFound(new { message = "User is not assigned to this consultant." });
            }

            // Видалення зв'язку між консультантом і користувачем
            _context.UserConsultants.Remove(userConsultant);

            // Оновлення кількості клієнтів консультанта
            var consultant = await _context.Consultants.FindAsync(consultantId);
            if (consultant != null)
            {
                consultant.current_clients -= 1;  // Зменшуємо кількість клієнтів
                _context.Entry(consultant).State = EntityState.Modified;
            }

            // Видалити всі активні запити цього користувача
            var activeRequests = await _context.ConsultantRequests
                .Where(cr => cr.user_uid == userId && cr.consultant_uid == consultantId && cr.status == "accepted")
                .ToListAsync();

            if (activeRequests.Any())
            {
                _context.ConsultantRequests.RemoveRange(activeRequests);  // Видаляємо активні запити
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "User removed successfully and associated requests deleted." });
        }

        [HttpPut("update-nickname/{consultantId}")]
        public async Task<IActionResult> UpdateConsultantNickname(string consultantId, [FromBody] UpdateConsultantNicknameRequest request)
        {
            var consultant = await _context.Consultants.FindAsync(consultantId);
            if (consultant == null)
            {
                return NotFound(new { message = "Consultant not found." });
            }

            // Оновлюємо лише нікнейм
            if (!string.IsNullOrEmpty(request.new_nickname))
            {
                consultant.nickname = request.new_nickname;
                _context.Entry(consultant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Nickname updated successfully." });
            }

            return BadRequest(new { message = "New nickname is required." });
        }

        [HttpPut("update-profile-picture/{consultantId}")]
        public async Task<IActionResult> UpdateConsultantProfilePicture(string consultantId, [FromBody] UpdateConsultantProfilePictureRequest request)
        {
            var consultant = await _context.Consultants.FindAsync(consultantId);
            if (consultant == null)
            {
                return NotFound(new { message = "Consultant not found." });
            }

            // Оновлюємо лише фото профілю
            if (!string.IsNullOrEmpty(request.new_profile_picture))
            {
                consultant.profile_picture = request.new_profile_picture;
                _context.Entry(consultant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile picture updated successfully." });
            }

            return BadRequest(new { message = "New profile picture is required." });
        }

        [HttpPut("update-profile-description/{consultantId}")]
        public async Task<IActionResult> UpdateConsultantProfileDescription(string consultantId, [FromBody] UpdateConsultantProfileDescriptionRequest request)
        {
            var consultant = await _context.Consultants.FindAsync(consultantId);
            if (consultant == null)
            {
                return NotFound(new { message = "Consultant not found." });
            }

            // Оновлюємо лише опис профілю
            if (!string.IsNullOrEmpty(request.new_profile_description))
            {
                consultant.profile_description = request.new_profile_description;
                _context.Entry(consultant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile description updated successfully." });
            }

            return BadRequest(new { message = "New profile description is required." });
        }

        [HttpPut("update-max-clients/{consultantId}")]
        public async Task<IActionResult> UpdateConsultantMaxClients(string consultantId, [FromBody] UpdateConsultantMaxClientsRequest request)
        {
            var consultant = await _context.Consultants.FindAsync(consultantId);
            if (consultant == null)
            {
                return NotFound(new { message = "Consultant not found." });
            }

            // Оновлюємо лише максимальну кількість клієнтів
            if (request.new_max_clients.HasValue)
            {
                consultant.max_clients = request.new_max_clients.Value;
                _context.Entry(consultant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Max clients updated successfully." });
            }

            return BadRequest(new { message = "New max clients count is required." });
        }



    }

    // DTOs
    public class InviteUserRequest
    {
        public string user_uid { get; set; }
    }

    public class RespondToInviteRequest
    {
        public string consultant_uid { get; set; }
        public bool is_accepted { get; set; }
    }

    public class UpdateConsultantNicknameRequest
    {
        public string new_nickname { get; set; }
    }

    public class UpdateConsultantProfilePictureRequest
    {
        public string new_profile_picture { get; set; }
    }

    public class UpdateConsultantProfileDescriptionRequest
    {
        public string new_profile_description { get; set; }
    }

    public class UpdateConsultantMaxClientsRequest
    {
        public int? new_max_clients { get; set; }
    }

}
