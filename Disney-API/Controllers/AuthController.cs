using Disney_API.Models;
using Disney_API.Services;
using Disney_API.Security;
using Microsoft.AspNetCore.Mvc;
using Disney_API.Models.Schemes;
using Microsoft.AspNetCore.Authorization;
namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DisneyContext? _context;

        public AuthController(DisneyContext context)
        {
            _context = context;
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
                return Ok(user);

            return Forbid("El usuario/clave es incorrecto");
            
        }
        
    }
}
