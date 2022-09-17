using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/actors")]
    [ApiController]
    public class ActorsController : ControllerBase
    {
        private readonly ILogger<ActorsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly string containerName = "actors";

        public ActorsController(ILogger<ActorsController> logger,
                               ApplicationDbContext context,
                               IMapper mapper,
                               IFileStorageService fileStorageService)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("searchByName")]
        public async Task<ActionResult<List<ActorMovieDTO>>> SerachByname([FromBody]string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new List<ActorMovieDTO>();
            }
            return await _context.Actors.Where(x => x.Name.Contains(name))
                                        .OrderBy(x => x.Name)
                                        .Select(x => new ActorMovieDTO { Id = x.Id, Name = x.Name, Picture = x.Picture })
                                        .Take(5).ToListAsync();
        }

        [HttpGet] // api/actors
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Actors.AsQueryable();
            await HttpContext.InsertParametersPaginationInHeader(queryable);
            var actors = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();

            return _mapper.Map<List<ActorDTO>>(actors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            return _mapper.Map<ActorDTO>(actor);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreationDTO);
            if (actorCreationDTO.Picture != null)
            {
                actor.Picture = await _fileStorageService.SaveFile(containerName, actorCreationDTO.Picture);
            }
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreationDTO actorCreationDTO)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            actor = _mapper.Map(actorCreationDTO, actor);
            if (actorCreationDTO.Picture != null)
            {
                actor.Picture = await _fileStorageService.EditFile(containerName,
                                      actorCreationDTO.Picture, actor.Picture);
            }

            await _context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            _context.Remove(actor);
            await _context.SaveChangesAsync();
            await _fileStorageService.DeleteFile(actor.Picture, containerName);
            return NoContent();
        }
    }
}
