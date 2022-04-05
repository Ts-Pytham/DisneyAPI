using Disney_API.ModelBinder;
using Disney_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

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

        [HttpGet()]
        public async Task<IActionResult> GetCharacters([FromQuery] GetRequestCharacters request)
        {
            if (_context == null)
                return NotFound();

            if (_context.Personajes.Any())
            {

                /* Conditions*/
                int cases = 0;
                if (request.Name != null) cases = 1;
                else if (request.Movies != null) cases = 2;
                else if (request.Age != null) cases = 3;

                if (request.Name != null && request.Age != null) cases = 4;
                else if (request.Name != null && request.Movies != null) cases = 5;
                else if (request.Age != null && request.Movies != null) cases = 6;
                else if (request.Name != null && request.Age != null && request.Movies != null) cases = 7;
                Debug.WriteLine(cases);
                if (cases == 0)
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
                else if (cases == 1)
                    return await GetPersonajesName(request.Name!);
                else if (cases == 2)
                {
                    return await GetPersonajesByIdMovie(request.Movies.GetValueOrDefault());


                }
                else if (cases == 3)
                {
                    
                    return await GetPersonajesAge(request.Age.GetValueOrDefault());
                }
                else
                {
                    
                    var task = (from p in _context.Personajes
                                join pa in _context.Participacions on p.Idpersonaje equals pa.Idpersonaje
                                where Conditions(p.Nombre, p.Edad, pa.Idpelicula, request, cases)
                                orderby p.Idpersonaje
                                select new
                                {
                                    Personaje = p,
                                    Pelicula = GetMovieByID(pa.Idpelicula, _context.Participacions.ToList(), _context.Peliculas.ToList())

                                }
                                ).ToListAsync();

                    var result = await task;
                    if(result == null || result.Count == 0)
                        return NotFound(result);

                    return Ok(result);
                }
            }

            return NotFound();
        }

        private static bool Conditions(string name, int age, int movies, GetRequestCharacters request, int cases)
        {

            if(cases == 4) return request.Name == name && request.Age == age;
            else if(cases == 5) return request.Name == name && request.Movies == movies;
            else if(cases == 6) return request.Age == age && request.Movies == movies;
            else if(cases == 7) return request.Name == name && request.Age == age && request.Movies == movies;
            return false;
 
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetCharactersDetails()
        {
            if (_context == null)
                return NotFound();
            var personajes = (from p in _context.Personajes
                              select new
                              {
                                  Personaje = p,
                                  Pelicula = GetMovieByID(p.Idpersonaje, _context.Participacions.ToList(), _context.Peliculas.ToList())

                              }
                              
                             ).ToListAsync();


            var result = await personajes;
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpGet("name/{nombre}")]
        public async Task<IActionResult> GetPersonajesName(string nombre)
        {
            if (_context == null)
                return NotFound();
            var personaje = _context.Personajes.OrderBy(x => x.Idpersonaje)
                            .Where(x => x.Nombre == nombre)
                            .Select(x => new
                            {
                                x.Nombre,
                                x.Edad,
                                x.Peso,
                                Peliculas = GetMovieByID(x.Idpersonaje, _context.Participacions.ToList(), _context.Peliculas.ToList())
                            })
                            .ToListAsync();

            var result = await personaje;
            if (result == null)
                return NotFound();

            return Ok(result);
        }
      
      
        [HttpGet("age/{edad}")]
        public async Task<IActionResult> GetPersonajesAge(int edad)
        {
            if (_context == null)
                return NotFound();

            var personajes = _context.Personajes.OrderBy(x => x.Edad)
                             .Where(x => x.Edad == edad)
                             .Select(x => new
                             {
                                 x.Nombre,
                                 x.Edad,
                                 x.Peso,
                                 Peliculas = GetMovieByID(x.Idpersonaje, _context.Participacions.ToList(), _context.Peliculas.ToList())
                              })
                             .ToListAsync();

            var result = await personajes;
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        
        [HttpGet("movies/{idMovie}")]
        public async Task<IActionResult> GetPersonajesByIdMovie(int idMovie)
        {
            if (_context == null)
                return NotFound();
            var personajes = (from p in _context.Participacions
                              join pe in _context.Personajes on p.Idpersonaje equals pe.Idpersonaje
                              join m in _context.Peliculas on p.Idpelicula equals m.Idpelicula
                              where m.Idpelicula == idMovie
                              select new {
                                  pe.Nombre, 
                                  pe.Edad,
                                  pe.Peso,
                                  Peliculas = new 
                                    {
                                      m.Titulo,
                                      m.Fecha,
                                      m.Calificacion,
                                      m.Imagen
                                    }
                                  }
                             ).ToListAsync();
                             

            var result = await personajes;
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        private static dynamic? GetMovieByID(int id, List<Participacion> participacions, List<Pelicula> peliculas)
        {

            var movies = (from p in participacions
                          join pe in peliculas on p.Idpelicula equals pe.Idpelicula
                          where p.Idpersonaje == id
                          select new { pe.Titulo, pe.Fecha, pe.Calificacion, pe.Imagen }).ToList();

            if (movies == null)
                return null;

            return movies;
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
