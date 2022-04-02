using Disney_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        #region GET

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
                foreach (var item in result)
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

        [HttpGet("{name}")]
        public async Task<IActionResult> GetPersonajesName(string name)
        {
            if (_context == null)
                return NotFound();
            var personaje = _context.Personajes.OrderBy(x => x.Nombre == name).ToListAsync();

            var result = await personaje;
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{age:int}")]
        public async Task<IActionResult> GetPersonajesAge(int age)
        {
            if (_context == null)
                return NotFound();

            var personajes = _context.Personajes.OrderBy(x => x.Edad == age)
                             .Select(x => new 
                             { 
                                 x.Nombre, 
                                 x.Edad, 
                                 x.Peso,
                                 Peliculas = _context.Participacions.Where(m => m.Idpersonaje == x.Idpersonaje)
                                             .Select(p => new { p.IdpeliculaNavigation}).ToList()
                                 
                             })
                             .ToListAsync();

            var result = await personajes;
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{movies}")]
        public async Task<IActionResult> GetPersonajesName(int movies)
        {
            if (_context == null)
                return NotFound();
            var personajes = _context.Participacions.OrderBy(x => x.Idpelicula == movies).ToListAsync();

            var result = await personajes;
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        #endregion

        #region POST
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

        #endregion

        #region PUT
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
        #endregion





    }
}
