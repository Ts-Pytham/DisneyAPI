using Disney_API.ModelBinder;
using Disney_API.Models;
using Disney_API.Models.Schemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;

namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CharactersController : ControllerBase
    {
        
        private readonly DisneyContext? _context;

        public CharactersController(DisneyContext context)
        {
            _context = context;
        }

        #region GET
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCharacters([FromQuery] GetRequestCharacters request)
        {
            if (_context == null)
                return NotFound();

            if (_context.Personajes.Any())
            {

                /* Conditions*/
                int cases = -1;
                if (request.Name != null && request.Movies == null && request.Age == null) cases = 1;
                else if (request.Movies != null && request.Name == null && request.Age == null) cases = 2;
                else if (request.Age != null && request.Name == null && request.Movies == null) cases = 3;
                else if (request.Name == null && request.Movies == null && request.Age == null) cases = 0;

                if (cases == 0)
                {
                    //_context.Personajes.OrderBy(x => x.Idpersonaje).ToListAsync();
                    var task = (from p in _context.Personajes
                                orderby p.Idpersonaje
                                select new
                                {
                                    p.Imagen,
                                    p.Nombre   
                                }).ToListAsync();

                    var result = await task;

                    if(result == null || result.Count == 0)
                        return NotFound();

                    
                    return Ok(result);
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
                    
                   var list1 = (from p in _context.Personajes  
                               orderby p.Idpersonaje 
                               where p.Nombre == request.Name                        
                               select new
                               {
                                   p.Idpersonaje,
                                   p.Nombre,
                                   p.Edad,
                                   p.Peso,

                               }
                               ).ToListAsync();

                    var result1 = await list1;

                    var list2 = (from p in _context.Personajes
                                 orderby p.Idpersonaje
                                 where p.Edad == request.Age
                                 select new
                                 {
                                     p.Idpersonaje,
                                     p.Nombre,
                                     p.Edad,
                                     p.Peso,

                                 }
                               ).ToListAsync();

                    var result2 = await list2;

                    var list3 = (from p in _context.Personajes
                                 orderby p.Idpersonaje
                                 join pa in _context.Participacions on p.Idpersonaje equals pa.Idpersonaje
                                 where pa.Idpelicula == request.Movies
                                 select new
                                 {
                                     p.Idpersonaje,
                                     p.Nombre,
                                     p.Edad,
                                     p.Peso,

                                 }
                               ).ToListAsync();
                    
                  
                    var result3 = await list3;
                    
                    if(request.Name == null)
                    {
                        result1 = result2;
                    }
                    if(request.Age == null)
                    {
                        result2 = result1;
                    }
                    if(request.Movies == null)
                    {
                        result3 = result2;
                    }

                    var Intersect = result1.Intersect(result2).Intersect(result3);
                    
                    if (!Intersect.Any())
                        return NotFound();

                    var result = (from p in Intersect
                                  select new
                                  {
                                      p.Nombre,
                                      p.Edad,
                                      p.Peso,
                                      Pelicula = GetMovieByID(p.Idpersonaje, _context.Participacions.ToList(), _context.Peliculas.ToList())
                                  }
                               ).ToList();

                    return Ok(result);

                }
            }

            return NotFound();
        }


        [AllowAnonymous]
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
            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }
        [AllowAnonymous]
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
            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
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
            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
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
            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        private static dynamic? GetMovieByID(int id, List<Participacion> participacions, List<Pelicula> peliculas)
        {

            var movies = (from p in participacions
                          join pe in peliculas on p.Idpelicula equals pe.Idpelicula
                          where p.Idpersonaje == id
                          select new 
                          {   
                              pe.Idpelicula,
                              pe.Imagen,
                              pe.Titulo, 
                              pe.Fecha, 
                              pe.Calificacion,
                              
                          }).ToList();

            if (movies == null)
                return null;

            return movies;
        }
        #endregion

        #region POST
        [HttpPost]
        public async Task<IActionResult> CreateCharacter([FromBody] CharacterCreate? personaje)
        {
            if (personaje == null || !ModelState.IsValid || _context == null)
                return BadRequest(ModelState);

            Personaje p = personaje;

            p.Idpersonaje = _context.Personajes.Count() + 1;
            var movies = personaje.IDMovies.Distinct().ToList();
            foreach(var movie in movies)
            {
                if(_context.Peliculas.Where(x => x.Idpelicula == movie).Any())
                {
                    await _context.AddAsync(new Participacion { Idpelicula = movie, Idpersonaje = p.Idpersonaje });
                    
                }
                    
            }
            await _context.AddAsync(p);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateCharacter), p);
        }

        #endregion

        #region PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCharacter(int id, [FromBody] CharacterUpdate character)
        {
            if (_context == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0 && _context.Personajes.Where(x => x.Idpersonaje == id).Any() == false)
                return NotFound("No se encontró el personaje");

            // Removing movies where characters.movies > 0 (possibly will changes in the future)
            
            if(character.IDMovies.Count > 0)
            {
                var list = _context.Participacions.Where(x => x.Idpersonaje == id).ToList();
                foreach(var movie in list)
                {
                    _context.Participacions.Remove(movie);
                }

                var movies = character.IDMovies.Distinct().ToList();
                foreach (var movie in movies)
                {
                    if (_context.Peliculas.Where(x => x.Idpelicula == movie).Any())
                    {
                        await _context.AddAsync(new Participacion { Idpelicula = movie, Idpersonaje = id });

                    }
                }
            }
            
            Personaje p = character;
            p.Idpersonaje = id;

            _context.Personajes.Update(p);
            await _context.SaveChangesAsync();
            return Ok(p);
            
        }
        #endregion


        #region DELETE      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacterById(int id)
        {
            if(_context == null)
                return BadRequest();
            var list = await _context.Personajes.Where(x => x.Idpersonaje == id).ToListAsync();
            var p = list.Count > 0 ? list[0] : null;
            if(p == null)
                return NotFound("No se encontró el personaje");

            _context.Personajes.Remove(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }
        #endregion


    }
}
