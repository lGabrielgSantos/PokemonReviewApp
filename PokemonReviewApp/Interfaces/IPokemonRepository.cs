using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        //this function provide for the client utilize
        ICollection<Pokemon> GetPokemons(); //return a pokemon list
    }
}
