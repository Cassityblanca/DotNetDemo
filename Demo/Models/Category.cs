using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Demo.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string? Name { get; set; }
        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "You must enter 1 to 100!")]
        public int DisplayOrder { get; set; }

    }

}
