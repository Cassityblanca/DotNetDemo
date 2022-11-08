using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Menu Item")]
        public string? Name { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater than $1")]
        public float Price { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public int CategoryId { get; set; }

        public int FoodTypeId { get; set; }

        // Connects to Objects or Tables
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("FoodTypeId")]
        public virtual FoodType? FoodType { get; set; }
    }
}
