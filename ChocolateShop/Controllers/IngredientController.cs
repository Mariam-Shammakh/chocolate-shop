using ChocolateShop.Data;
using ChocolateShop.Models;
using ChocolateShop.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChocolateShop.Controllers
{
    public class IngredientController : Controller
    {
        private readonly IIngredientService _ingredientService;
        private readonly IProductService _productService;
       

        public IngredientController(IIngredientService ingredientService,  IProductService productService)
        {
            _ingredientService = ingredientService;
            
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            return View(ingredients);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _productService.GetAllProductsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingredient ingredient, List<int> productIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _productService.GetAllProductsAsync();
                return View(ingredient);
            }

            await _ingredientService.CreateIngredientAsync(ingredient, productIds);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            ViewBag.Products = await _productService.GetAllProductsAsync();
            ViewBag.SelectedProducts = ingredient.Products.Select(p => p.Id).ToList();

            return View(ingredient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ingredient ingredient, List<int> productIds)
        {
            if (id != ingredient.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _productService.GetAllProductsAsync();
                return View(ingredient);
            }

            await _ingredientService.UpdateIngredientAsync(ingredient, productIds);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ingredientService.DeleteIngredientAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
