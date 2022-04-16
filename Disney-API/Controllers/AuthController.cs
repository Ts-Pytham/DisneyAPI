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
using Disney_API.Utilities;
using System.Diagnostics;
using DotEnv.Core;

namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DisneyContext? _context;
        private readonly ITokenService tokenService;
        public AuthController(DisneyContext context, IConfiguration configuration)
        {
            _context = context;
            tokenService = new TokenService();
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

            string EmailFrom = "johansanchezdeleon@gmail.com";
            string UserEmailFrom = "DISNEY-API";
            string EmailTo = user.Email;
            string UserEmailTo = "Usuario";
            string PlainTextContent = "";
            string HtmlContent = "Bienvenido a Disney-API, recuerda guardar la clave dada, " +
                                 "para conseguir el token debe de iniciar sesión. " +
                                 "El token se vence cada 4 horas.\n" +
                                 $"Token: {tokenService.GetToken(user.Email)}";
            string Subject = "DISNEY API, Registro exitoso";
            var reader = new EnvReader();
            
            var send = new Utilities.SendGrid(reader["SENDGRID_API_KEY"], EmailFrom, UserEmailFrom, UserEmailTo, Subject, EmailTo, PlainTextContent, HtmlContent); ;
            await send.SendEmail();
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
                
                return Ok(tokenService.GetToken(user.Email));
            }

            return Forbid();
            
        }
        
    }
}
