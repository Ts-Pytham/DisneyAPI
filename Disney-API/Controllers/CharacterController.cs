using Disney_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        
        private readonly DisneyContext? _context;

        public CharactersController(DisneyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonajes()
        {
            if (_context == null)
                return NotFound();

            if (_context.Personajes.Any())
            {
                var task = _context.Personajes.OrderBy(x => x.Idpersonaje).ToListAsync();

                var result = await task;

                List<Character> list = new();
                foreach(var item in result)
                {
                    list.Add(new Character
                    {
                        Imagen = item.Imagen,
                        Nombre = item.Nombre
                    });
                }

                return Ok(list);
            }

            return NotFound();
        }

        [HttpGet("{nombre}")]
        public async Task<IActionResult> GetPersonajeNombre(string nombre)
        {
            if (_context == null)
                return NotFound();
            var personaje = _context.Personajes.OrderBy(x => x.Nombre == nombre).ToListAsync();

            var result = await personaje;
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{character}")]
        public async Task<IActionResult> GetPersonaje(Personaje character)
        {
            if (_context == null)
                return NotFound();

            var data = _context.Personajes.Where(x => x.Idpersonaje == character.Idpersonaje || x.Imagen == character.Imagen || x.Peso == character.Peso || x.Edad == character.Edad).ToListAsync();

            var result = await data;
            if (result == null)
                return NotFound();

            return Ok(result);

        }

        [HttpPost]
        public async Task<IActionResult> CrearPersonaje([FromBody] CharacterCreate? personaje)
        {
            if (personaje == null || !ModelState.IsValid || _context == null)
                return BadRequest(ModelState);

            Personaje p = personaje;

            p.Idpersonaje = _context.Personajes.Count() + 1;

            await _context.AddAsync(p);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CrearPersonaje), p);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPersonaje(int id, [FromBody] CharacterUpdate character)
        {
            if (_context == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0 && _context.Personajes.Where(x => x.Idpersonaje == id).Any() == false)
                return BadRequest();

            Personaje p = character;
            p.Idpersonaje = id;
             _context.Personajes.Update(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }
        
    }
}
