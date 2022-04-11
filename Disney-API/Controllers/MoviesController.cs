using Disney_API.Models;
using Disney_API.Models.Schemes;
using Disney_API.ModelBinder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly DisneyContext? _context;

        public MoviesController(DisneyContext context)
        {
            _context = context;
        }

        #region GET
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery] GetRequestMovies request)
        {
            if (_context == null)
                return NotFound();

            if (_context.Peliculas.Any())
            {
                int cases = 0;
                if (request.Name != null && request.Genre == null) cases = 1;
                else if (request.Name == null && request.Genre != null) cases = 2;
                else if (request.Name != null && request.Genre != null) cases = 3;

                request.Order = request.Order.ToUpper();

                if (request.Order != "ASC" && request.Order != "DESC")
                    request.Order = "ASC";

                if (cases == 0)
                {
                    var task = _context.Peliculas.OrderBy(x => x.Idpelicula)
                                .Select(x => new
                                {
                                    x.Imagen,
                                    x.Titulo,
                                    x.Fecha
                                })
                                .ToListAsync();

                    var result = await task;
                    if (result.Count == 0 || result == null)
                        return NotFound();

                    if (request.Order == "ASC")
                    {
                        result = result.OrderBy(x => x.Fecha).ToList();
                    }
                    else
                    {
                        result = result.OrderByDescending(x => x.Fecha).ToList();
                    }

                    return Ok(result);
                }
                else if (cases == 1)
                    return await GetMoviesByName(request?.Name!, request?.Order!);
                else if (cases == 2)
                    return await GetMoviesByGenre(request.Genre.GetValueOrDefault(), request?.Order!);
                else
                {
                    var list1 = (from p in _context.Peliculas
                                 where p.Titulo == request.Name
                                 select new
                                 {
                                      p.Idpelicula,
                                      p.Imagen,
                                      p.Titulo,
                                      p.Fecha,
                                      p.Calificacion

                                 }).ToListAsync();

                    var result1 = await list1;

                    var list2 = (from p in _context.Peliculas
                                 join gp in _context.GeneroPeliculas on p.Idpelicula equals gp.Idpelicula
                                 where gp.Idgenero == request.Genre.GetValueOrDefault()
                                 select new
                                 {
                                     p.Idpelicula,
                                     p.Imagen,
                                     p.Titulo,
                                     p.Fecha,
                                     p.Calificacion

                                 }).Distinct().ToListAsync();

                    var result2 = await list2;
                    if(request.Name == null)
                    {
                        result1 = result2;
                    }
                    if(request.Genre == null)
                    {
                        result2 = result1;
                    }

                    var Intersect = result1.Intersect(result2).ToList();

                    if (!Intersect.Any())
                        return NotFound();

                    var result = (from p in Intersect
                                  select new
                                  {
                                      p.Idpelicula,
                                      p.Imagen,
                                      p.Titulo,
                                      p.Fecha,
                                      p.Calificacion,
                                      Personaje = GetCharacterById(p.Idpelicula, _context.Participacions.ToList(), _context.Personajes.ToList()),
                                      Genero = GetGenresById(p.Idpelicula, _context.GeneroPeliculas.ToList(), _context.Generos.ToList())
                                  }
                               ).ToList();
                    if (request.Order == "ASC")
                        result = result.OrderBy(x => x.Fecha).ToList();
                    else
                        result = result.OrderByDescending(x => x.Fecha).ToList();

                    return Ok(result);

                }
            }

            return NotFound();
        }
        [AllowAnonymous]
        [HttpGet("details")]
        public async Task<IActionResult> GetMoviesDetails()
        {
            if (_context == null)
                return NotFound();
            var movies = (from p in _context.Peliculas     
                              select new
                              {
                                  Pelicula = p,
                                  Personaje = GetCharacterById(p.Idpelicula,_context.Participacions.ToList(), _context.Personajes.ToList()),
                                  Genero = GetGenresById(p.Idpelicula, _context.GeneroPeliculas.ToList(), _context.Generos.ToList())
                              }

                             ).ToListAsync();


            var result = await movies;
            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }
        [AllowAnonymous]
        [HttpGet("name/{nombre}")]
        public async Task<IActionResult> GetMoviesByName(string nombre, string order = "ASC")
        {
            if (_context == null)
                return NotFound();
            var movies = (from p in _context.Peliculas
                          where p.Titulo == nombre
                          select new
                          {
                              Pelicula = p,
                              Personaje = GetCharacterById(p.Idpelicula, _context.Participacions.ToList(), _context.Personajes.ToList()),
                              Genero = GetGenresById(p.Idpelicula, _context.GeneroPeliculas.ToList(), _context.Generos.ToList())
                          }).ToListAsync();

            var result = await movies;
            if (order == "ASC")
                result = result.OrderBy(x => x.Pelicula.Fecha).ToList();
            else
                result = result.OrderByDescending(x => x.Pelicula.Fecha).ToList();

            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("genre/{idGenero}")]
        public async Task<IActionResult> GetMoviesByGenre(int idGenero, string order = "ASC")
        {
            if (_context == null)
                return BadRequest();

            var list = (
                            from p in _context.Peliculas
                            join g in _context.GeneroPeliculas on p.Idpelicula equals g.Idpelicula
                            where g.Idgenero == idGenero
                            select new
                            {
                                Pelicula = p,
                                Personaje = GetCharacterById(p.Idpelicula, _context.Participacions.ToList(), _context.Personajes.ToList()),
                                Genero = GetGenresById(p.Idpelicula, _context.GeneroPeliculas.ToList(), _context.Generos.ToList())
                            }

                       ).ToListAsync();
            var result = await list;

            if (order == "ASC")
                result = result.OrderBy(x => x.Pelicula.Fecha).ToList();
            else
                result = result.OrderByDescending(x => x.Pelicula.Fecha).ToList();

            if (list == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }
        private static dynamic? GetCharacterById(int id, List<Participacion> participacions, List<Personaje> personajes)
        {
            var list = (from pe in participacions
                        join c in personajes on pe.Idpersonaje equals c.Idpersonaje
                        where pe.Idpelicula == id
                        select new
                        {
                            c.Imagen,
                            c.Nombre,
                            c.Edad,
                            c.Peso,
                            c.Historia
                        }
                        ).ToList();

            return list ?? null;

        }
        private static dynamic? GetGenresById(int id, List<GeneroPelicula> generoPeliculas, List<Genero> generos)
        {
            var list = (from ge in generoPeliculas
                        join g in generos on ge.Idgenero equals g.Idgenero
                        where ge.Idpelicula == id
                        orderby ge.Idgenero
                        select new
                        {
                            g.Idgenero,
                            g.Imagen,
                            g.Nombre

                        }
                        ).ToList();

            return list ?? null;
        }
        #endregion

        #region POST
        [HttpPost]
        public async Task<IActionResult> CreateMovie([FromBody] MovieCreate? movie)
        {
            if (movie == null || !ModelState.IsValid || _context == null)
                return BadRequest(ModelState);

            Pelicula p = movie;

            p.Idpelicula = _context.Peliculas.Count() + 1;
            
            var listc = movie.IDCharacters.Distinct().ToList();
            var listg = movie.IDGenres.Distinct().ToList();
            foreach(var idcharacter in listc)
            {
                if(_context.Personajes.Where(x => x.Idpersonaje == idcharacter).Any())
                {
                    await _context.AddAsync(new Participacion { Idpelicula = p.Idpelicula, Idpersonaje = idcharacter });
                }
            }
            
            foreach(var idgenre in listg)
            {
                if (_context.Generos.Where(x => x.Idgenero == idgenre).Any())
                {
                    await _context.AddAsync(new GeneroPelicula { Idpelicula = p.Idpelicula, Idgenero = idgenre });
                }
            }
            
            await _context.AddAsync(p);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateMovie), p);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieUpdate movie)
        {
            if (_context == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (id <= 0 && _context.Peliculas.Where(x => x.Idpelicula == id).Any() == false)
                return BadRequest();

            Pelicula p = movie;
            p.Idpelicula = id;

            // Removing movies where movie.IDCharacters > 0 (possibly will changes in the future)

            if (movie.IDCharacters.Count > 0)
            {
                var list = _context.Participacions.Where(x => x.Idpelicula == id).ToList();
                foreach (var character in list)
                {
                    _context.Participacions.Remove(character);
                }

                var characters = movie.IDCharacters.Distinct().ToList();
                foreach (var idcharacter in characters)
                {
                    if (_context.Personajes.Where(x => x.Idpersonaje == idcharacter).Any())
                    {
                        await _context.AddAsync(new Participacion { Idpelicula = id, Idpersonaje = idcharacter });
                    }
                }
            }

            // Removing movies where movie.IDGenres > 0 (possibly will changes in the future)
            if(movie.IDGenres.Count > 0)
            {
                var list = _context.GeneroPeliculas.Where(x => x.Idpelicula == id).ToList();
                foreach (var genre in list)
                {
                    _context.GeneroPeliculas.Remove(genre);
                }

                var genres = movie.IDGenres.Distinct().ToList();
                foreach (var idgenre in genres)
                {
                    if (_context.Generos.Where(x => x.Idgenero == idgenre).Any())
                    {
                        await _context.AddAsync(new GeneroPelicula { Idpelicula = id, Idgenero = idgenre });
                    }
                }
            }
            _context.Peliculas.Update(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            if (_context == null)
                return BadRequest();

            if (_context.Peliculas.Any())
            {
                var task = _context.Peliculas.Where(x => x.Idpelicula == id)
                            .FirstOrDefaultAsync();

                var result = await task;
                if (result == null)
                    return NotFound();
                _context.Peliculas.Remove(result);
                await _context.SaveChangesAsync();
                return Ok(result);
            }

            return NotFound();
        }
        #endregion
    }

    
}
