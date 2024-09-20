using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class CategoryPostDTO
    {
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The category name must be between 3 and 100 characters.")]
        public string Name { get; set; }
    }
}
