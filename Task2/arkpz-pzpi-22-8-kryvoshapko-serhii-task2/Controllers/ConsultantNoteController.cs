using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NutriTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultantNoteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConsultantNoteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add-note")]
        public async Task<IActionResult> AddNote([FromBody] CreateConsultantNoteRequest request)
        {
            // Отримуємо ціль разом з користувачем
            var goal = await _context.UserGoals
                .Include(g => g.User) // Завантажуємо користувача, пов'язаного з цією метою
                .FirstOrDefaultAsync(g => g.goal_id == request.goal_id);

            if (goal == null)
            {
                return NotFound(new { message = "Goal not found." });
            }

            // Перевіряємо статус запиту консультанта
            var consultantRequest = await _context.ConsultantRequests
                .FirstOrDefaultAsync(cr =>
                    cr.consultant_uid == request.consultant_uid &&
                    cr.user_uid == goal.User.user_uid && // Використовуємо goal.User.user_uid
                    cr.status == "accepted");

            if (consultantRequest == null)
            {
                return BadRequest(new { message = "Consultation request must be accepted." });
            }

            // Створення нотатки
            var note = new ConsultantNote
            {
                consultant_uid = request.consultant_uid,
                goal_id = request.goal_id,
                content = request.content,
                created_at = DateTime.UtcNow,
                user_uid = goal.User.user_uid // Додаємо user_uid
            };

            _context.ConsultantNotes.Add(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Note added successfully." });
        }


        [HttpPut("update-note/{noteId}")]
        public async Task<IActionResult> UpdateNote(int noteId, [FromBody] UpdateConsultantNoteRequest request)
        {
            var note = await _context.ConsultantNotes
                .Include(n => n.UserGoal)
                .ThenInclude(g => g.User)
                .FirstOrDefaultAsync(n => n.note_id == noteId);

            if (note == null)
            {
                return NotFound(new { message = "Note not found." });
            }

            // Перевіряємо права та статус запиту
            var consultantRequest = await _context.ConsultantRequests
                .FirstOrDefaultAsync(cr =>
                    cr.consultant_uid == request.consultant_uid &&
                    cr.user_uid == note.UserGoal.User.user_uid &&
                    cr.status == "accepted");

            if (consultantRequest == null)
            {
                return BadRequest(new { message = "Consultation request must be accepted." });
            }

            if (note.consultant_uid != request.consultant_uid)
            {
                return Unauthorized(new { message = "You are not authorized to update this note." });
            }

            note.content = request.content;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Note updated successfully." });
        }

        [HttpGet("get-notes/{goalId}")]
        public async Task<IActionResult> GetNotes(int goalId)
        {
            var notes = await _context.ConsultantNotes
                .Include(n => n.Consultant)
                .Where(n => n.goal_id == goalId)
                .OrderByDescending(n => n.created_at)
                .Select(n => new
                {
                    n.note_id,
                    consultant_nickname = n.Consultant.nickname,
                    n.consultant_uid,
                    n.goal_id,
                    n.content,
                    n.created_at
                })
                .ToListAsync();

            if (!notes.Any())
            {
                return NotFound(new { message = "No notes found for this goal." });
            }

            return Ok(notes);
        }

        [HttpDelete("delete-note/{noteId}")]
        public async Task<IActionResult> DeleteNote(int noteId, [FromQuery] string consultantUid)
        {
            var note = await _context.ConsultantNotes
                .Include(n => n.UserGoal)
                .ThenInclude(g => g.User)
                .FirstOrDefaultAsync(n => n.note_id == noteId);

            if (note == null)
            {
                return NotFound(new { message = "Note not found." });
            }

            // Перевіряємо права та статус запиту
            var consultantRequest = await _context.ConsultantRequests
                .FirstOrDefaultAsync(cr =>
                    cr.consultant_uid == consultantUid &&
                    cr.user_uid == note.UserGoal.User.user_uid &&
                    cr.status == "accepted");

            if (consultantRequest == null)
            {
                return BadRequest(new { message = "Consultation request must be accepted." });
            }

            if (note.consultant_uid != consultantUid)
            {
                return Unauthorized(new { message = "You are not authorized to delete this note." });
            }

            _context.ConsultantNotes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Note deleted successfully." });
        }
    }

    public class CreateConsultantNoteRequest
    {
        [Required]
        public string consultant_uid { get; set; }

        [Required]
        public int goal_id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string content { get; set; }
    }

    public class UpdateConsultantNoteRequest
    {
        [Required]
        public string consultant_uid { get; set; }

        [Required]
        [MaxLength(1000)]
        public string content { get; set; }
    }
}