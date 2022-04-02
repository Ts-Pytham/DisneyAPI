using Disney_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Disney_API.Controllers
{
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
                var task = _context.Peliculas.OrderBy(x => x.Idpelicula).ToListAsync();

                var result = await task;

                List<Movie> list = new();
                foreach (var item in result)
                {
                    list.Add(new Movie
                    {
                        Imagen = item.Imagen,
                        Titulo = item.Titulo,
                        Fecha = item.Fecha
                    });
                }

                return Ok(list);
            }

            return NotFound();
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

        #region
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
