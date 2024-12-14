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
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.user_uid == request.user_uid);

            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            // Оновлення last_login тільки датою
            user.last_login = DateTime.Today;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.user_uid,
                user.nickname,
            });
        }

        [HttpPost("login-consultant")]
        public async Task<IActionResult> LoginConsultant([FromBody] ConsultantLoginRequest request)
        {
            var consultant = await _context.Consultants.FirstOrDefaultAsync(c => c.consultant_uid == request.consultant_uid);

            if (consultant == null)
            {
                return Unauthorized(new { message = "Consultant not found." });
            }

            // Оновлення last_login тільки датою
            consultant.last_login = DateTime.Today;
            _context.Entry(consultant).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                consultant.consultant_uid,
                consultant.nickname,
                consultant.profile_picture,
                consultant.profile_description
            });
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.user_uid == request.user_uid))
            {
                return BadRequest(new { message = "User already registered." });
            }

            var user = new User
            {
                user_uid = request.user_uid,
                nickname = request.nickname,
                profile_picture = request.profile_picture,
                profile_description = request.profile_description,
                gender = request.gender,
                height = request.height,
                current_weight = request.current_weight,
                created_at = DateTime.Today, // Зберігаємо тільки дату
                last_login = DateTime.Today, // Зберігаємо тільки дату
                is_active = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Login), new { FirebaseUid = user.user_uid }, user);
        }

        [HttpPost("register/consultant")]
        public async Task<IActionResult> RegisterConsultant([FromBody] RegisterConsultantRequest request)
        {
            if (await _context.Consultants.AnyAsync(c => c.consultant_uid == request.consultant_uid))
            {
                return BadRequest(new { message = "Consultant already registered." });
            }

            var consultant = new Consultant
            {
                consultant_uid = request.consultant_uid,
                nickname = request.nickname,
                profile_picture = request.profile_picture,
                profile_description = request.profile_description,
                experience_years = request.experience_years,
                max_clients = request.max_clients,
                created_at = DateTime.Today, // Зберігаємо тільки дату
                last_login = DateTime.Today, // Зберігаємо тільки дату
                is_active = true
            };

            _context.Consultants.Add(consultant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(LoginConsultant), new { FirebaseUid = consultant.consultant_uid }, consultant);
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
        {
            if (await _context.Admins.AnyAsync(a => a.admin_uid == request.admin_uid))
            {
                return BadRequest(new { message = "Admin already registered." });
            }

            var admin = new Admin
            {
                admin_uid = request.admin_uid,
                registration_date = DateTime.Today, // Зберігаємо тільки дату
                name = request.name,
                email = request.email,
                phone_number = request.phone_number
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(LoginAdmin), new { adminUid = admin.admin_uid }, admin);
        }

        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] AdminLoginRequest request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.admin_uid == request.admin_uid);

            if (admin == null)
            {
                return Unauthorized(new { message = "Admin not found." });
            }

            return Ok(new
            {
                admin.admin_uid,
                admin.name,
                admin.email,
                admin.phone_number
            });
        }

    }

    public class LoginRequest
    {
        public string user_uid { get; set; }
    }

    public class RegisterUserRequest
    {
        public string user_uid { get; set; }
        public string nickname { get; set; }
        public string profile_picture { get; set; }
        public string profile_description { get; set; }
        public string gender { get; set; }
        public int height { get; set; }
        public float current_weight { get; set; }
        public float TargetWeight { get; set; }
        public int DurationWeeks { get; set; }
    }

    public class RegisterConsultantRequest
    {
        public string consultant_uid { get; set; }
        public string nickname { get; set; }
        public string profile_picture { get; set; }
        public string profile_description { get; set; }
        public int experience_years { get; set; }
        public int max_clients { get; set; }
    }

    public class ConsultantLoginRequest
    {
        public string consultant_uid { get; set; }
    }

    public class RegisterAdminRequest
    {
        public string admin_uid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
    }

    public class AdminLoginRequest
    {
        public string admin_uid { get; set; }
    }

}