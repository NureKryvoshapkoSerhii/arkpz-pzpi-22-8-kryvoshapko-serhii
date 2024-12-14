using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System;
using System.Threading.Tasks;

namespace NutriTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GoalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(request.user_uid);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                if (!user.current_weight.HasValue || !user.height.HasValue || string.IsNullOrEmpty(user.gender))
                {
                    return BadRequest(new { message = "User data is incomplete for goal creation." });
                }

                // Calculate nutrition needs using the user's gender
                var (calories, protein, carbs, fats) = CalculateNutrition(
                    currentWeight: user.current_weight.Value,
                    targetWeight: request.target_weight,
                    durationWeeks: request.duration_weeks,
                    height: user.height.Value,
                    gender: user.gender, // Getting gender from the user data in the database
                    goalType: request.goal_type
                );

                var goal = new UserGoal
                {
                    user_uid = request.user_uid,
                    consultant_uid = request.consultant_uid,
                    goal_type = request.goal_type,
                    target_weight = request.target_weight,
                    duration_weeks = request.duration_weeks,
                    daily_calories = calories,
                    daily_protein = protein,
                    daily_carbs = carbs,
                    daily_fats = fats,
                    start_date = DateTime.UtcNow,
                    is_approved_by_consultant = request.consultant_uid == null
                };

                _context.UserGoals.Add(goal);
                await _context.SaveChangesAsync();

                var response = MapToGoalResponse(goal);
                return CreatedAtAction(nameof(GetGoal), new { id = goal.goal_id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Get specific goal
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGoal(int id)
        {
            var goal = await _context.UserGoals
                .Include(g => g.User)
                .Include(g => g.Consultant)
                .FirstOrDefaultAsync(g => g.goal_id == id);

            if (goal == null)
            {
                return NotFound(new { message = "Goal not found." });
            }

            var response = MapToGoalResponse(goal);
            return Ok(response);
        }

        [HttpGet("user/{uid}")]
        public async Task<IActionResult> GetUserGoals(string uid)
        {
            try
            {
                var goals = await _context.UserGoals
                    .Include(g => g.User)
                    .Include(g => g.Consultant)
                    .Where(g => g.user_uid == uid)
                    .OrderByDescending(g => g.start_date)  // Сортуємо за датою, найновіші перші
                    .ToListAsync();

                if (!goals.Any())
                {
                    return NotFound(new { message = "No goals found for this user." });
                }

                var response = goals.Select(MapToGoalResponse).ToList();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // Update user weight and recalculated goal if weight changed
        [HttpPut("update-weight/{userId}")]
        public async Task<IActionResult> UpdateUserWeight(string userId, [FromBody] UpdateWeightRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Update user weight
            user.current_weight = request.new_weight;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Now, update goal based on the new weight
            var userGoal = await _context.UserGoals
                .FirstOrDefaultAsync(g => g.user_uid == userId && !g.is_approved_by_consultant);

            if (userGoal != null)
            {
                // Recalculate nutrition needs with the new weight
                var (calories, protein, carbs, fats) = CalculateNutrition(
                    currentWeight: user.current_weight.Value,
                    targetWeight: userGoal.target_weight,
                    durationWeeks: userGoal.duration_weeks,
                    height: user.height.Value,
                    gender: user.gender,
                    goalType: userGoal.goal_type
                );

                // Update the goal with new values
                userGoal.daily_calories = calories;
                userGoal.daily_protein = protein;
                userGoal.daily_carbs = carbs;
                userGoal.daily_fats = fats;

                _context.Entry(userGoal).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Weight updated and goal recalculated successfully." });
            }

            return NotFound(new { message = "Goal not found for the user." });
        }

        [HttpPut("approve-goal/{goalId}")]
        public async Task<IActionResult> ApproveGoal(int goalId, [FromBody] ApproveGoalRequest request)
        {
            try
            {
                var goal = await _context.UserGoals
                    .FirstOrDefaultAsync(g => g.goal_id == goalId && g.consultant_uid == request.consultant_uid);

                if (goal == null)
                {
                    return NotFound(new { message = "Goal not found or consultant not authorized to approve this goal." });
                }

                // Оновлення статусу підтвердження
                goal.is_approved_by_consultant = true;

                _context.Entry(goal).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Goal successfully approved." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private GoalResponse MapToGoalResponse(UserGoal goal)
        {
            return new GoalResponse
            {
                goal_id = goal.goal_id,
                user_uid = goal.user_uid,
                consultant_uid = goal.consultant_uid,
                goal_type = goal.goal_type,
                target_weight = goal.target_weight,
                duration_weeks = goal.duration_weeks,
                daily_calories = goal.daily_calories,
                daily_protein = goal.daily_protein,
                daily_carbs = goal.daily_carbs,
                daily_fats = goal.daily_fats,
                start_date = goal.start_date,
                is_approved_by_consultant = goal.is_approved_by_consultant,
                user = goal.User != null ? new UserBasicInfo
                {
                    user_uid = goal.User.user_uid,
                    nickname = goal.User.nickname,
                    profile_picture = goal.User.profile_picture
                } : null,
                consultant = goal.Consultant != null ? new ConsultantBasicInfo
                {
                    user_uid = goal.Consultant.consultant_uid,
                    nickname = goal.Consultant.nickname,
                    profile_picture = goal.Consultant.profile_picture
                } : null
            };
        }

        // Calculate nutrition needs
        private (double calories, double protein, double carbs, double fats) CalculateNutrition(
             double currentWeight,
             double targetWeight,
             int durationWeeks,
             int height,
             string gender,
            GoalType goalType)
        {
            double bmr;

            // Використовуємо різні формули для чоловіків та жінок
            if (gender.ToLower() == "male")
            {
                bmr = (10 * currentWeight) + (6.25f * height) - (5 * 30) + 5;  // Для чоловіків
            }
            else
            {
                bmr = (10 * currentWeight) + (6.25f * height) - (5 * 30) - 161; // Для жінок
            }

            double tdee = bmr * 1.55f; 

            double dailyCalories = tdee;
            switch (goalType)
            {
                case GoalType.Loss:
                    dailyCalories = tdee - 500;
                    break;
                case GoalType.Gain:
                    dailyCalories = tdee + 300;
                    break;
                case GoalType.Maintain:
                    dailyCalories = tdee;
                    break;
            }

            // Розрахунок макроелементів
            double protein = currentWeight * 2.0f; // 2 г на 1 кг ваги
            double fats = (dailyCalories * 0.25f) / 9; // 25% калорій
            double carbs = (dailyCalories - (protein * 5) - (fats * 9)) / 4;

            return (dailyCalories, protein, carbs, fats);
        }


        // DTOs
        public class CreateGoalRequest
        {
            public string user_uid { get; set; }
            public string? consultant_uid { get; set; }
            public GoalType goal_type { get; set; }
            public double target_weight { get; set; }
            public int duration_weeks { get; set; }
        }

        public class GoalResponse
        {
            public int goal_id { get; set; }
            public string user_uid { get; set; }
            public string? consultant_uid { get; set; }
            public GoalType goal_type { get; set; }
            public double target_weight { get; set; }
            public int duration_weeks { get; set; }
            public double daily_calories { get; set; }
            public double daily_protein { get; set; }
            public double daily_carbs { get; set; }
            public double daily_fats { get; set; }
            public DateTime start_date { get; set; }
            public bool is_approved_by_consultant { get; set; }
            public UserBasicInfo user { get; set; }
            public ConsultantBasicInfo consultant { get; set; }
        }

        public class UserBasicInfo
        {
            public string user_uid { get; set; }
            public string nickname { get; set; }
            public string profile_picture { get; set; }
        }

        public class ConsultantBasicInfo
        {
            public string user_uid { get; set; }
            public string nickname { get; set; }
            public string profile_picture { get; set; }
        }

        public class UpdateWeightRequest
        {
            public double new_weight { get; set; }
        }

        public class ApproveGoalRequest
        {
            public string consultant_uid { get; set; }
        }
    }
}