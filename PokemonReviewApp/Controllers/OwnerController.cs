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
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public OwnerController(IMapper mapper, IOwnerRepository ownerRepository, ICountryRepository countryRepository)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
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

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
                                          //FromQuery permit pass data for url(id)
        public IActionResult CreatedOwner([FromQuery] int coutryId, OwnerDto ownerCreated) 
        {
            if(ownerCreated == null)
                return BadRequest(ModelState);

            var ownerRepeat = _ownerRepository.GetOwners()
                .Where(r => r.LastName.Trim().ToUpper() == ownerCreated.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();
            
            if(ownerRepeat != null)
            {
                ModelState.AddModelError("", "Owner exist");
                return StatusCode(400, ModelState);
            }

            if(!ModelState.IsValid) 
                return BadRequest();

            var ownerMap = _mapper.Map<Owner>(ownerCreated);
            ownerMap.Country = _countryRepository.GetCountry(coutryId);

            var sucess = _ownerRepository.CreatedOwner(ownerMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Error in saved");
                return StatusCode(500, ModelState);
            }

            return Ok("Owner created!");
        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int ownerId, int coutryId, [FromBody] OwnerDto updateOwner)
        {
            if(updateOwner == null)
                return BadRequest();

            if(ownerId != updateOwner.Id)
                return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExists(ownerId))
                return BadRequest();

            if (!ModelState.IsValid)
                return NotFound();

            var ownerMap = _mapper.Map<Owner>(updateOwner);
            ownerMap.Country = _countryRepository.GetCountry(coutryId);

            var sucess = _ownerRepository.UpdateOwner(ownerMap);

            if(!sucess) 
            {
                ModelState.AddModelError("", "Error in updatee.");
                return StatusCode(500, ModelState);
            }

            return Ok("Owner update");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if(ownerId == null) 
                return BadRequest();

            if(!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var ownerDelete = _ownerRepository.GetOwner(ownerId);

            if(!ModelState.IsValid)
                return BadRequest();

            var sucess = _ownerRepository.DeleteOwner(ownerDelete);

            if(!sucess)
            {
                ModelState.AddModelError("", "Error in delete owner");
                return StatusCode(500, ModelState);
            }
            return Ok("Owner Deleted");
        }

    }
}
