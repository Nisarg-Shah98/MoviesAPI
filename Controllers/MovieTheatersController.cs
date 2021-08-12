using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/movieTheaters")]
    [ApiController]
    public class MovieTheatersController : ControllerBase
    {
        private readonly ILogger<MovieTheatersController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MovieTheatersController(ILogger<MovieTheatersController> logger,
                               ApplicationDbContext context,
                               IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;

        }

        [HttpGet] 
        public async Task<List<MovieTheaterDTO>> Get()
        {
            var entities = await _context.MovieTheaters.OrderBy(x => x.Name).ToListAsync();

            return _mapper.Map<List<MovieTheaterDTO>>(entities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovieTheaterDTO>> Get(int id)
        {
            var movieTheater = await _context.MovieTheaters.FirstOrDefaultAsync(x => x.Id == id);
            if (movieTheater == null)
            {
                return NotFound();
            }
            return _mapper.Map<MovieTheaterDTO>(movieTheater);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MovieTheaterCreationDTO theaterCreationDTO)
        {
            try
            {
                var movieTheater = _mapper.Map<MovieTheater>(theaterCreationDTO);
                _context.MovieTheaters.Add(movieTheater);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] MovieTheaterCreationDTO theaterCreationDTO)
        {
            var movieTheater = await _context.MovieTheaters.FirstOrDefaultAsync(x => x.Id == id);
            if (movieTheater == null)
            {
                return NotFound();
            }
            movieTheater = _mapper.Map(theaterCreationDTO, movieTheater);
            await _context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movieTheater = await _context.MovieTheaters.FirstOrDefaultAsync(x => x.Id == id);
            if (movieTheater == null)
            {
                return NotFound();
            }
            _context.Remove(movieTheater);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
