using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Entities
{
    public class Category
    {
        public int Id {  get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido.")]
        [StringLength(100,ErrorMessage ="El campo {0} no puede tener mas de {1} caracteres.")]
        public string? Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryProduct> Products { get; set; } = new List<CategoryProduct>();

    }
}
