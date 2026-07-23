using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.DTOs.Products
{
    public class ProductUpdateDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? Name { get; set; }
        [StringLength(maximumLength: 200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? Description { get; set; }
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? SKU { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser un número negativo.")]
        public int Stock { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo.")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public IFormFile? ImageUrl { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public List<int> CategoryIds { get; set; } = new List<int>();


    }
}
