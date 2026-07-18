using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Api.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? Name { get; set; }
        [StringLength(maximumLength: 200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? Description { get; set; }
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? SKU { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "El campo {0} no puede ser un número negativo.")]
        public int Stock { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "El campo {0} no puede ser negativo.")]
        public decimal Price { get; set; }
        
        [StringLength(500, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryProduct> Categories { get; set; }= new List<CategoryProduct>();


    }
}
