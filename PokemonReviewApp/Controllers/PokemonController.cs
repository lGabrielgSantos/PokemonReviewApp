using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")] //pass route
    [ApiController] //i speak with a api controller
    public class PokemonController : Controller
    {
        //controller get the data with geting in the repository and show the public

        private readonly IPokemonRepository _pokemonRepository; //call my interface
        private readonly DataContext context; // call my context


        //istance my controller
        public PokemonController(IPokemonRepository pokemonRepository, DataContext context)
        {
            _pokemonRepository = pokemonRepository;
            this.context = context;
        }


        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))] //my response is a list the pokemons
        public IActionResult GetPokemons()
        {
            var pokemons = _pokemonRepository.GetPokemons(); //pokemons is a result the getPokemons in the my interface
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

    }
}
