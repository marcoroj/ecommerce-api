using ECommerce.Api.Data;
using ECommerce.Api.DTOs;
using ECommerce.Api.DTOs.Categories;
using ECommerce.Api.DTOs.Products;
using ECommerce.Api.Entities;
using ECommerce.Api.Utils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ECommerce.Api.Services
{
    public class ProductService:IProductService
    {
        private const string contenedor = "productos-imagenes";

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;


        public ProductService(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos
            ,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _almacenadorArchivos = almacenadorArchivos;
            _contextAccessor= httpContextAccessor;
        }

        public async Task<ProductResponseDto> CreateAsync(ProductCreateDto productCreateDto)
        {
            if (productCreateDto.CategoryIds is null || productCreateDto.CategoryIds.Count == 0)
            {
                throw new ArgumentException("No se puede crear producto sin categoria",nameof(productCreateDto.CategoryIds));
                
            }

            var categoriesDb = await _context.Categories
                .Where(c => productCreateDto.CategoryIds.Contains(c.Id))
                .Select(x => new { x.Id, x.Name }).ToListAsync();

            if (categoriesDb.Count != productCreateDto.CategoryIds.Count)
            {
                var categoriesExists = categoriesDb.Select(x => x.Id).ToList();

                var categoriesNoExists = productCreateDto.CategoryIds.Except(categoriesExists);
                var categoriesNoExistsString = string.Join(",", categoriesNoExists);

                throw new ArgumentException(
                    $"Los siguientes ids de categorias no existen:{categoriesNoExistsString}");
            }

            var product = new Product
            {
                Name = productCreateDto.Name,
                Description = productCreateDto.Description,
                SKU = productCreateDto.SKU,
                // Redondeamos a 2 decimales usando el "redondeo comercial" (hacia el número más lejano de cero)
                Price = Math.Round(productCreateDto.Price, 2, MidpointRounding.AwayFromZero),
                //ImageUrl = productCreateDto.ImageUrl,
                Categories = productCreateDto.CategoryIds
                .Select(x => new CategoryProduct { CategoryId = x }).ToList(),
                Stock = productCreateDto.Stock,
            };

            if (productCreateDto.ImageUrl is not null)
            {
                var url = await _almacenadorArchivos.Almacenar(contenedor, productCreateDto.ImageUrl);
                product.ImageUrl = url;

            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Stock = product.Stock,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                Categories = categoriesDb
                .Select(x => new CategoryResponseDto
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList()
            };

            return productDto;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(ProductFiltroDto productFiltroDto,PaginationDto paginationDto)
        {
            var queryable = _context.Products
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(productFiltroDto.Name))
            {
                queryable = queryable.Where(x => x.Name.Contains(productFiltroDto.Name));
            }

            if (productFiltroDto.StockMenorACinco.HasValue)
            {
                if (productFiltroDto.StockMenorACinco.Value)
                {
                    queryable = queryable.Where(x => x.Stock < 5);
                }
            }

            if (productFiltroDto.TieneImagen.HasValue)
            {
                if (productFiltroDto.TieneImagen.Value)
                {
                    queryable = queryable.Where(x => x.ImageUrl != null);
                }
                else
                {
                    queryable = queryable.Where(x => x.ImageUrl == null);
                }

            }

            if (productFiltroDto.PrecioMinimo.HasValue
                && productFiltroDto.PrecioMaximo.HasValue)
            {
                var priceMin = productFiltroDto.PrecioMinimo.Value;
                var priceMax = productFiltroDto.PrecioMaximo.Value;
                queryable = queryable
                    .Where(x => x.Price >= priceMin && x.Price <= priceMax);
            }
            if (!string.IsNullOrEmpty(productFiltroDto.NombreCategoria))
            {
                queryable = queryable
                    .Where(x => x.Categories
                    .Any(x =>
                    x.Category!.Name == productFiltroDto.NombreCategoria));
            }

            await _contextAccessor.HttpContext.InsertarPaginacionHeader(queryable);


            var response = await queryable
                .Include(x => x.Categories)
                .ThenInclude(x => x.Category)
                .OrderBy(x => x.Id)
                .Paginate(paginationDto)
                .ToListAsync();

            var productsDto = response
                .Select(prod => new ProductResponseDto
                {
                    Id = prod.Id,
                    Name = prod.Name,
                    Description = prod.Description,
                    SKU = prod.SKU,
                    Stock = prod.Stock,
                    Price = prod.Price,
                    ImageUrl = prod.ImageUrl,
                    CreatedAt = prod.CreatedAt,
                    Categories = prod.Categories
                    .Select(x => new CategoryResponseDto
                    {
                        Id = x.CategoryId,
                        Name = x.Category?.Name
                    }).ToList()
                });
            return productsDto;

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

       

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto productUpdateDto)
        {
            var product = await _context.Products
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return false;
            }

            if (productUpdateDto.CategoryIds is null || productUpdateDto.CategoryIds.Count == 0)
            {
                throw new ArgumentException("No se puede crear producto sin categoria");
                
            }

            var categoryIdsExists = await _context.Categories
                .Where(x => productUpdateDto.CategoryIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (categoryIdsExists.Count != productUpdateDto.CategoryIds.Count)
            {
                var categoriesNoExists = productUpdateDto.CategoryIds.Except(categoryIdsExists);
                var categoriesStringNoExists = string.Join(",", categoriesNoExists);

                throw new ArgumentException(
                    $"Las siguientes categorias no existe:{categoriesStringNoExists}");
               
            }

            if (productUpdateDto.ImageUrl is not null)
            {
                var url = await _almacenadorArchivos.RemplazarArchivo
                    (product.ImageUrl, contenedor, productUpdateDto.ImageUrl);
                product.ImageUrl = url;
            }

            product.Name = productUpdateDto.Name;
            product.Description = productUpdateDto.Description;
            product.Price = productUpdateDto.Price;
            product.SKU = productUpdateDto.SKU;
            product.Stock = productUpdateDto.Stock;
            product.Categories = productUpdateDto.CategoryIds
                .Select(x => new CategoryProduct { CategoryId = x })
                .ToList();

            await _context.SaveChangesAsync();
            return true;
            
        }


        public async Task<ProductPatchDto?> GetPatchDtoForUpdate(int id)
        {
            var product=await _context.Products
                .Include(pcl=>pcl.Categories)
                .ThenInclude(pc=>pc.Category)
                .FirstOrDefaultAsync(x=>x.Id== id);
            if(product is null)
            {
                return null;
            }
            var productDto = new ProductPatchDto
            {
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Stock = product.Stock,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryIds = product.Categories
                .Select(x => x.CategoryId).ToList(),

            };
            return productDto;

        }
        public async Task<bool> UpdatePatchAsync(int id, ProductPatchDto productPatchDto)
        {
            var product = await _context.Products
               .Include(x => x.Categories)
               .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return false;
            }

            // Verificando si las categorias existen en la DB
            if (productPatchDto.CategoryIds is null || productPatchDto.CategoryIds.Count == 0)
            {
                throw new ArgumentException("No se puede actualizar producto sin categoria",
                    nameof(productPatchDto.CategoryIds));
                
            }

            var categoryIdsExists = await _context.Categories
                .Where(x => productPatchDto.CategoryIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (categoryIdsExists.Count != productPatchDto.CategoryIds.Count)
            {
                var categoriesNoExists = productPatchDto.CategoryIds.Except(categoryIdsExists);
                var categoriesStringNoExists = string.Join(",", categoriesNoExists);
                throw new ArgumentException(
                    $"Las siguientes categorias no existe:{categoriesStringNoExists}"
                    , nameof(productPatchDto.CategoryIds));
                
            }

            //actualizando el state de la entidad producto obtenida de la db

            product.Name = productPatchDto.Name;
            product.Description = productPatchDto.Description;
            product.Price = productPatchDto.Price;
            product.ImageUrl = productPatchDto.ImageUrl;
            product.SKU = productPatchDto.SKU;
            product.Stock = productPatchDto.Stock;
            product.Categories = productPatchDto.CategoryIds
                .Select(x => new CategoryProduct { CategoryId = x }).ToList();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteLogicAsync(int id)
        {
            var product = await _context.Products
                .Where(x=>x.IsActive)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null)
            {
                return false;
            }
            product.IsActive = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
