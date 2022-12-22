using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")] //pass route
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;
        public OwnerController(IMapper mapper, IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if (!ModelState.IsValid)
                return NotFound();

            return Ok(owners);
        }
        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        public IActionResult GetOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return BadRequest();

            var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid)
                return NotFound();
            return Ok(owner);
        }

        [HttpGet("/owner/{pokeId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwnerOfAPokemon(int pokeId)
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwnerOfAPokemon(pokeId));
            if (!ModelState.IsValid)
                return NotFound();

            return Ok(owners);

        }

        [HttpGet("/pokemon/{ownerId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
                return BadRequest();

            var pokemons = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));

            if (!ModelState.IsValid)
                return NotFound();

            return Ok(pokemons);

        }



    }
}
