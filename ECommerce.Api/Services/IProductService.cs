using ECommerce.Api.DTOs;
using ECommerce.Api.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(ProductFiltroDto productFiltroDto, PaginationDto paginationDto);
        Task<ProductResponseDto?> GetByIdAync(int id);
        Task<ProductResponseDto> CreateAsync(ProductCreateDto productCreateDto);
        Task<bool> UpdateAsync(int id,ProductUpdateDto productUpdateDto);
    }
}
