using Disney_API.Models;
using Disney_API.Services;
using Disney_API.Security;
using Microsoft.AspNetCore.Mvc;
using Disney_API.Models.Schemes;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DisneyContext? _context;
        private readonly IConfiguration _configuration;
        public AuthController(DisneyContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreate user)
        {
            if (user == null || !ModelState.IsValid || _context == null)
                return BadRequest(ModelState);

            UserService service = new(_context);
            
            user.Password = ComputeHash.ToSHA512(user.Password);

            if (await service.IsUser(user))
                return BadRequest("El usuario ya existe!");

            Usuario usuario = user;
            usuario.Id = _context.Usuarios.Count() + 1;

            await _context.AddAsync(usuario); 
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), user);

        }
  
        [HttpPost("login")]
        
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (user == null || !ModelState.IsValid || _context == null)
                return BadRequest(ModelState);

            UserService service = new(_context);

            user.Password = ComputeHash.ToSHA512(user.Password);

            if (await service.IsUser(user))
            {
                var secretKey = _configuration.GetValue<string>("SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey!);

                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Email));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(12), //12 horas de validez del token
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var createdToken = tokenHandler.CreateToken(tokenDescriptor);

                string bearer_token = tokenHandler.WriteToken(createdToken);
                return Ok(bearer_token);
            }

            return Forbid();
            
        }
        
    }
}
