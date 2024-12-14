using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;
using System.Text.Json.Serialization;

namespace NutriTrack
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
            .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                 options.JsonSerializerOptions.MaxDepth = 64; // Можна налаштувати максимальну глибину
             });

            // Додайте реєстрацію ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Використовуємо рядок підключення з appsettings.json

            // Конфігурація Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
