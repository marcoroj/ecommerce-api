using ECommerce.Api.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAync(int id);
        Task<ActionResult> CreateAsync(ProductCreateDto productCreateDto);
    }
}
