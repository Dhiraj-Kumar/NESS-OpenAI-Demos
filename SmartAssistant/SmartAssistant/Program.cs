
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenAI;
using OpenAI.Chat;
using SmartAssistant.Services;
using System.Text;
using Serilog;

namespace SmartAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/smart-assistant-logs.txt")
                .CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddSingleton<OpenAIClient>(options =>
            {
                return new OpenAIClient(builder.Configuration["OPEN_API_KEY"]);
            });
            builder.Services.AddSingleton<ChatClient>(options =>
            {
                var openAIClient = options.GetRequiredService<OpenAIClient>();
                return openAIClient.GetChatClient(builder.Configuration["AI_MODEL"]);
            });
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IChatAssistant, ChatAssistant>();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_my_secret_key_for_this_application"));
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = "assistantapp",

                ValidateAudience = true,
                ValidAudience = "user",

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication().UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
