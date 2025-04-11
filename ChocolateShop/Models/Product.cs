using System.ComponentModel.DataAnnotations;

namespace ChocolateShop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        [Display(Name ="Image")]
        public  string? ProductImage { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        [Display(Name = "Ingredients")]
        public virtual ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    }
}
