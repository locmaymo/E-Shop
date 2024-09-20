using System.ComponentModel.DataAnnotations;

namespace Catalog.API.DTOs
{
    public class ProductGetDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Weight { get; set; }
        public int? Volume { get; set; }
        public double? UnitPrice { get; set; }
        public int? Discount { get; set; }
        public string? ImageURL { get; set; }


        public bool? IsActice { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class ProductPutDTO
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Weight { get; set; }
        public int? Volume { get; set; }
        public double? UnitPrice { get; set; }
        public int? Discount { get; set; }
        public string? ImageURL { get; set; }


        public bool? IsActice { get; set; }
    }

    public class ProductPostDTO
    {

        public string? Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Description must be between 1 and 200 characters.")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Weight must be greater than or equal to 0.")]
        public int? Weight { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Volume must be greater than or equal to 0.")]
        public int? Volume { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Unit Price must be greater than or equal to 0.")]
        public double? UnitPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public int? Discount { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Image must be between 1 and 200 characters.")]
        public string? ImageURL { get; set; }


        public bool? IsActice { get; set; }
        [Required]
        public int? CategoryId { get; set; }
    }
}
