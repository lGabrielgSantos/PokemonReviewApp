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
    }
}
