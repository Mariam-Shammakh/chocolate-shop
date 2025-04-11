using ChocolateShop.Data;
using ChocolateShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ChocolateShop.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddIngredientsToProductAsync(int productId, List<int> ingredientIds);
        Task<Product> CreateProductAsync(Product product, IFormFile? imageFile, List<int> ingredientIds);
        Task<Product?> UpdateProductAsync(Product product, IFormFile? imageFile, List<int> ingredientIds);
        Task<bool> DeleteProductAsync(int id);
        
    }


    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Ingredients).Include(e => e.Category).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products.Include(p => p.Ingredients).Include(e => e.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddIngredientsToProductAsync(int productId, List<int> ingredientIds)
        {
            var product = await _context.Products
                .Include(p => p.Ingredients)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            //Console.WriteLine("Ingredient IDs in service:");
            //foreach (var id in ingredientIds)
            //{
            //    Console.WriteLine(id);  // Log each ingredient ID
            //}

            // Query ingredients by IDs
            var ingredients = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .ToListAsync();

            if (!ingredients.Any())
            {
                throw new Exception("No valid ingredients found.");
            }

            product.Ingredients.Clear();

            foreach (var ingredient in ingredients)
            {
                product.Ingredients.Add(ingredient);
            }

            await _context.SaveChangesAsync();
        }




        public async Task<Product> CreateProductAsync(Product product, IFormFile imageFile, List<int> ingredientIds)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                product.ProductImage = await SaveImageAsync(imageFile);
            }

            // Attach the selected ingredients
            product.Ingredients = await _context.Ingredients.Where(i => ingredientIds.Contains(i.Id)).ToListAsync();

            _context.Products.Add(product);
            
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(Product product, IFormFile? imageFile, List<int> ingredientIds)
        {
            var existingProduct = await _context.Products
                                                .Include(p => p.Ingredients)
                                                .FirstOrDefaultAsync(p => p.Id == product.Id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(existingProduct.ProductImage))
                {
                    DeleteImage(existingProduct.ProductImage);
                }
                existingProduct.ProductImage = await SaveImageAsync(imageFile);
            }

            // Update ingredients
            existingProduct.Ingredients.Clear();
            existingProduct.Ingredients = await _context.Ingredients.Where(i => ingredientIds.Contains(i.Id)).ToListAsync();

            await _context.SaveChangesAsync();
            return existingProduct;
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(product.ProductImage))
            {
                DeleteImage(product.ProductImage);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

            // Create the directory if it does not exist
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Generate a unique file name and save the file
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Verify the file path
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("File path is invalid.");
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Return relative path to store in the database
            return $"/images/{uniqueFileName}";
        }



        private void DeleteImage(string imagePath)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/", imagePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

}
