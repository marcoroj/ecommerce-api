using ECommerce.Api.DTOs;
using ECommerce.Api.DTOs.Products;

namespace ECommerce.Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync(ProductFiltroDto productFiltroDto, PaginationDto paginationDto);
        Task<ProductResponseDto?> GetByIdAync(int id);
        Task<ProductResponseDto> CreateAsync(ProductCreateDto productCreateDto);
        Task<bool> UpdateAsync(int id,ProductUpdateDto productUpdateDto);
        Task<bool> UpdatePatchAsync(int id,ProductPatchDto patchDocument);
        Task<ProductPatchDto?> GetPatchDtoForUpdate(int id);
        Task<bool> DeleteLogicAsync(int id);
    }
}
