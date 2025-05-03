using Microsoft.AspNetCore.Mvc;
using FacturaSii.src.Auth.Shared.DTOs;
using FacturaSii.src.Auth.Interfaces;

namespace FacturaSii.src.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            // TODO: validate user credentials (e.g., check against database)
            if (login.Username != "admin" || login.Password != "password")
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _tokenService.GenerateToken(login.Username);
            return Ok(new { Token = token });
        }
    }
}
