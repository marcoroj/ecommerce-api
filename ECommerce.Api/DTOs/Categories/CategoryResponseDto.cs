using ECommerce.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.DTOs.Categories
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
