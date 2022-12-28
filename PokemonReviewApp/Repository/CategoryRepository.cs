using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _context;

        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public bool CategoryExist(int categoryId)
        {
            return _context.Categories.Any(c => c.Id == categoryId);
        }

        public bool CreateCategory(Category category)
        {
            _context.Add(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.Id).ToList();
        }
        public Category GetCategory(int categoryId)
        {
            return _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

        }
        public Category GetCategory(string categoryName)
        {
            return _context.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
        }
        public ICollection<Pokemon> GetPokemonByCategoryId(int categoryId) // n - n
        {
            //                 search in PokemonCategories for id
            return _context.PokemonCategories.Where(e => e.CategoryId ==categoryId) //get pokemons with category id
                .Select(c => c.Pokemon).ToList(); // return pokemons with categoryId
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();

            return saved > 0 ? true : false;

        }
    }
}
