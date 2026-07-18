using ECommerce.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.DTOs.Categories
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [StringLength(100, ErrorMessage = "El campo {0} no puede tener mas de {1} caracteres.")]
        public string? Name { get; set; }
        
    }
}
