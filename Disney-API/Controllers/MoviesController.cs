using Disney_API.Models;
using Disney_API.Models.Schemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetMovies()
        {
            if (_context == null)
                return NotFound();

            if (_context.Peliculas.Any())
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
                return Ok(result);
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
        public async Task<IActionResult> GetMoviesByName(string nombre)
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
            if (result == null || result.Count == 0)
                return NotFound();

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("genre/{idGenero}")]
        public async Task<IActionResult> GetMoviesByGenre(int idGenero)
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
    }
}
