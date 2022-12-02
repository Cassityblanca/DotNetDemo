using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Demo.Models;

public class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    public int MenuItemId { get; set; }
    [ForeignKey("MenuItemId")]
    [ValidateNever]
    public MenuItem MenuItem { get; set; }

    [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Count { get; set; }

    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]

    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }

    [NotMapped]
    public double Price { get; set; }
}
