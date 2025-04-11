using ChocolateShop.Models;
using ChocolateShop.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocolateShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IIngredientService _ingredientService;
        private readonly ICategoryService _categoryService;
        

        public ProductController(IProductService productService,  IIngredientService ingredientService, ICategoryService categoryService)
        {
            _productService = productService;
            _ingredientService = ingredientService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile, List<int> ingredientIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
                return View(product);
            }
            if (imageFile != null && imageFile.Length > 0)
            {
                
                foreach (var id in ingredientIds)
                {
                    Console.WriteLine(id);  // Log each ingredient ID
                }

                // Proceed with saving the product and ingredients
                var createdProduct = await _productService.CreateProductAsync(product, imageFile,ingredientIds);

                try
                {
                    await _productService.AddIngredientsToProductAsync(createdProduct.Id, ingredientIds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Log exception for troubleshooting
                    throw; // Re-throw to see the error in detail
                }

            }
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
            ViewBag.SelectedIngredients = product.Ingredients.Select(i => i.Id).ToList();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile imageFile, List<int> ingredientIds)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Ingredients = await _ingredientService.GetAllIngredientsAsync();
                return View(product);
            }
            if (imageFile != null && imageFile.Length > 0)
            {

                foreach (var idd in ingredientIds)
                {
                    Console.WriteLine(idd);  // Log each ingredient ID
                }

                // Proceed with saving the product and ingredients
                var updatedProduct = await _productService.UpdateProductAsync(product, imageFile, ingredientIds);
                if (updatedProduct == null)
                {
                    return NotFound();
                }

                try
                {
                    await _productService.AddIngredientsToProductAsync(updatedProduct.Id, ingredientIds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Log exception for troubleshooting
                    throw; // Re-throw to see the error in detail
                }

            }
            

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
