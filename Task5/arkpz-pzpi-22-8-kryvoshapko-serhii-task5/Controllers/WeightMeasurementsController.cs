using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NutriTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightMeasurementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WeightMeasurementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/WeightMeasurements
        [HttpPost]
        public async Task<IActionResult> PostWeightMeasurement([FromBody] WeightMeasurementRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid data.");
            }

            // Перевіряємо чи існує користувач із таким user_uid
            var user = await _context.Users.FirstOrDefaultAsync(u => u.user_uid == request.UserUid);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Створюємо новий об'єкт вимірювання ваги
            var weightMeasurement = new WeightMeasurement
            {
                user_uid = request.UserUid,
                weight = request.Weight,
                measured_at = request.MeasuredAt,
                device_id = request.DeviceId,
                is_synced = request.IsSynced
            };

            // Додаємо в базу даних
            _context.WeightMeasurements.Add(weightMeasurement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeightMeasurement), new { id = weightMeasurement.measurement_id }, weightMeasurement);
        }

        // GET: api/WeightMeasurements/{userUid}
        [HttpGet("user/{userUid}")]
        public async Task<ActionResult<IQueryable<WeightMeasurement>>> GetWeightMeasurementsByUserUid(string userUid)
        {
            // Отримуємо всі вимірювання для цього користувача
            var weightMeasurements = await _context.WeightMeasurements
                .Where(wm => wm.user_uid == userUid)
                .ToListAsync();

            if (weightMeasurements == null || !weightMeasurements.Any())
            {
                return NotFound("No weight measurements found for this user.");
            }

            return Ok(weightMeasurements);
        }

        // GET: api/WeightMeasurements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WeightMeasurement>> GetWeightMeasurement(int id)
        {
            var weightMeasurement = await _context.WeightMeasurements.FindAsync(id);

            if (weightMeasurement == null)
            {
                return NotFound();
            }

            return weightMeasurement;
        }
    }

    // Клас для запиту (Body) у POST запиті
    public class WeightMeasurementRequest
    {
        public string UserUid { get; set; }
        public double Weight { get; set; }
        public DateTime MeasuredAt { get; set; }
        public string DeviceId { get; set; }
        public bool IsSynced { get; set; }
    }
}
