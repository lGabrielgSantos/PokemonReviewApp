using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")] //pass route
    [ApiController] //i speak with a api controller
    public class PokemonController : Controller
    {
        //controller get the data with geting in the repository and show the public

        private readonly IPokemonRepository _pokemonRepository; //call my interface
        private readonly IOwnerRepository _ownerRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        
        //istance my controller
        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper, IReviewRepository reviewRepository)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _reviewRepository= reviewRepository;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))] //my response is a list the pokemons
        public IActionResult GetPokemons()
        {
                                         //mapper for view with data for my dto
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons()); //pokemons is a result the getPokemons in the my interface

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]             //type with return
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));
            if(pokemon == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
                return BadRequest();
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreated)
        {
            if(pokemonCreated == null)
            {
                return BadRequest(ModelState);
            }

            var pokemonExist = _pokemonRepository.GetPokemons()
                .Where(p => p.Name.ToUpper().Trim() == pokemonCreated.Name.ToUpper().TrimEnd())
                .FirstOrDefault();

            if(pokemonExist != null)
            {
                ModelState.AddModelError("", "Pokemon exist!");
                return StatusCode(400, ModelState);
            }

            if(!ModelState.IsValid) return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreated);

            var sucess = _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Something went wrong while savin");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId, [FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto updatePokemon)
        {
            if (updatePokemon == null)
                return BadRequest();

            if (pokeId != updatePokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return BadRequest();

            if (!ModelState.IsValid)
                return NotFound();

            var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);

            
            var sucess = _pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMap);

            if (!sucess)
            {
                ModelState.AddModelError("", "Error in updatee.");
                return StatusCode(500, ModelState);
            }

            return Ok("Owner update");
        }
        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);

            //check reviews of pokemon
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

            //delete all reviews of pokemon
            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong when deleting reviews");
                return StatusCode(500, ModelState);
            }

            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong when deleting pokemon");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon deleted!");
        }
    }
}
