using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context; //set you context

        //get database with entity franework, we permit manipulate data 
        public PokemonRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Pokemon> GetPokemons()
        {
            //entry in the context select table POKEMON orden for id and list with list, ever pass details for call
            return _context.Pokemon.OrderBy(p => p.Id).ToList();
        }
        public Pokemon GetPokemon(int id)
        {
            //                       verify id and return the first 
            return _context.Pokemon.Where(p => p.Id == id).FirstOrDefault();
        }
        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemon.Where(p => p.Name == name).FirstOrDefault();
        }
        public decimal GetPokemonRating(int pokeId) 
        {
            //                                  verify in review the id of pokemon = search id
            var review = _context.Reviews.Where(r => r.Pokemon.Id == pokeId);

            if(review.Count() <= 0)
                return 0;
            //      convert result, soma the ratings and divide for total of reviews
            return ((decimal)review.Sum(r => r.Rating)) / review.Count();
        }

        public bool PokemonExists(int id)
        {
            //     if meet a pokemon with id, return true
            return _context.Pokemon.Any(p => p.Id == id);
        }
    }
}
