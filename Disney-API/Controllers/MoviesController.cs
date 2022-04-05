using Disney_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly DisneyContext? _context;

        public MoviesController(DisneyContext context)
        {
            _context = context;
        }

        #region GET

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

                return Ok(result);
            }

            return NotFound();
        }

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
            if (result == null)
                return NotFound();

            return Ok(result);
        }

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
            _context.Peliculas.Update(p);
            await _context.SaveChangesAsync();
            return Ok(p);
        }
        #endregion
    }
}
