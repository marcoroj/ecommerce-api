using ECommerce.Api.Data;
using ECommerce.Api.DTOs.Categories;
using ECommerce.Api.DTOs.Products;
using ECommerce.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ECommerce.Api.Services
{
    public class ProductService:IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> CreateAsync(ProductCreateDto productCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponseDto?> GetByIdAync(int id)
        {
            var productDb=await _context.Products
                .Include(pc=>pc.Categories)
                .ThenInclude(c=>c.Category)
                .FirstOrDefaultAsync(p=>p.Id==id);
            
            if(productDb is null)
            {
                return null;
            }

            var productDto = new ProductResponseDto
            {
                Id = productDb.Id,
                Name = productDb.Name,
                Description = productDb.Description,
                SKU = productDb.SKU,
                Stock = productDb.Stock,
                Price = productDb.Price,
                ImageUrl = productDb.ImageUrl,
                CreatedAt = productDb.CreatedAt,
                Categories = productDb.Categories
                .Select(x => new CategoryResponseDto
                {
                    Id = x.CategoryId,
                    Name = x.Category?.Name

                }).ToList()
            };

            return productDto;

        }
    }
}
