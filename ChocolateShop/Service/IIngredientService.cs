using ChocolateShop.Data;
using ChocolateShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateShop.Service
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient?> GetIngredientByIdAsync(int id);
        Task<Ingredient> CreateIngredientAsync(Ingredient ingredient, List<int> productIds);
        Task<Ingredient?> UpdateIngredientAsync(Ingredient ingredient, List<int> productIds);
        Task<bool> DeleteIngredientAsync(int id);
    }

    public class IngredientService : IIngredientService
    {
        private readonly AppDbContext _context;

        public IngredientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _context.Ingredients.Include(i => i.Products).ToListAsync();
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(int id)
        {
            return await _context.Ingredients.Include(i => i.Products).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Ingredient> CreateIngredientAsync(Ingredient ingredient, List<int> productIds)
        {
            ingredient.Products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            _context.Ingredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<Ingredient?> UpdateIngredientAsync(Ingredient ingredient, List<int> productIds)
        {
            var existingIngredient = await _context.Ingredients.Include(i => i.Products).FirstOrDefaultAsync(i => i.Id == ingredient.Id);
            if (existingIngredient == null)
            {
                return null;
            }

            existingIngredient.Name = ingredient.Name;
            existingIngredient.Description = ingredient.Description;
            existingIngredient.Products.Clear();
            existingIngredient.Products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            await _context.SaveChangesAsync();
            return existingIngredient;
        }

        public async Task<bool> DeleteIngredientAsync(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return false;
            }

            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }

        
    }

}
