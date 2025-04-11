using System.ComponentModel.DataAnnotations;

namespace ChocolateShop.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ProductId { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
