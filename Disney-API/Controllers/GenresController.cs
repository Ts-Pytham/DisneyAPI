using Disney_API.Models;
using Disney_API.Models.Interfaces;
using Disney_API.Models.Schemes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Disney_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenresController : ControllerBase
    {
        private readonly DisneyContext _context;

        public GenresController(DisneyContext context)
        {
            _context = context;
        }

        #region GET

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGenres()
        {
            if (_context == null)
                return NotFound();

            if (_context.Generos.Any())
            {
                var task = _context.Generos.OrderBy(x => x.Idgenero)
                            .Select(x => new
                            {
                                x.Idgenero,
                                x.Imagen,
                                x.Nombre
                                
                            })
                            .ToListAsync();

                var result = await task;
                if (result.Count == 0 || result == null)
                    return NotFound();
                return Ok(result);
            }

            return NotFound();
        }

        
        [HttpGet("id/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGenreById(int id)
        {
            if (_context == null)
                return NotFound();

            if (_context.Generos.Any())
            {
                var task = _context.Generos.Where(x => x.Idgenero == id)
                            .Select(x => new
                            {
                                x.Idgenero,
                                x.Imagen,
                                x.Nombre
                            })
                            .FirstOrDefaultAsync();

                var result = await task;
                if (result == null)
                    return NotFound();
                return Ok(result);
            }

            return NotFound();
        }
        #endregion

        #region POST
        [HttpPost]
        
        public async Task<IActionResult> CreateGenre([FromBody] GenreCreate genre)
        {
            if (_context == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            Genero g = genre;
            g.Idgenero = _context.Generos.Count() + 1;
            _context.Generos.Add(g);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGenre), g);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreUpdate genre)
        {
            if (_context == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_context.Generos.Where(x => x.Idgenero == id).Any())
                return NotFound("No se encontro el genero");

            Genero g = genre;
            g.Idgenero = id;
            _context.Generos.Update(g);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(UpdateGenre), g);
        }
        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            if (_context == null)
                return BadRequest();

            if (_context.Generos.Any())
            {
                var task = _context.Generos.Where(x => x.Idgenero == id)
                            .FirstOrDefaultAsync();

                var result = await task;
                if (result == null)
                    return NotFound();
                _context.Generos.Remove(result);
                await _context.SaveChangesAsync();
                return Ok(result);
            }

            return NotFound();
        }
        #endregion
    }
}
