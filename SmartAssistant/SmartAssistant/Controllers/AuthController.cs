using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SmartAssistant.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartAssistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login(User user)
        {
            if(user.email == "dhiraj@gmail.com" &&  user.password == "123456")
            {
                var claims = new[]
                {
                    new Claim("email", user.email)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this_is_my_secret_key_for_this_application"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "assistantapp",
                    audience: "user",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                var response = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                };

                return Ok(JsonConvert.SerializeObject(response));
            }
            return Unauthorized();
        }
    }
}
