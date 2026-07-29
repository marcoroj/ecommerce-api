using ECommerce.Api.Data;
using ECommerce.Api.DTOs;
using ECommerce.Api.DTOs.Categories;
using ECommerce.Api.DTOs.Products;
using ECommerce.Api.Entities;
using ECommerce.Api.Services;
using ECommerce.Api.Utils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private const string contenedor = "productos-imagenes";
        private readonly IProductService _productService;

        public ProductsController(IHttpContextAccessor httpContextAccessor
            , IAlmacenadorArchivos almacenadorArchivos, IProductService productService)
        {

            _contextAccessor = httpContextAccessor;
            _almacenadorArchivos = almacenadorArchivos;
            _productService = productService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get([FromQuery] PaginationDto paginationDto)
        //{
        //    var queryable = _context.Products.Where(x => x.IsActive).AsQueryable();

        //    await _contextAccessor.HttpContext.InsertarPaginacionHeader(queryable);

        //    var response = await queryable
        //        .Include(x => x.Categories)
        //        .ThenInclude(x => x.Category)
        //        .OrderBy(x => x.Id)
        //        .Paginate(paginationDto)
        //        .ToListAsync();

        //    //var products = await _context.Products
        //    //    .Where(p => p.IsActive)
        //    //    .Include(p => p.Categories)
        //    //    .ThenInclude(p=>p.Category)
        //    //    .ToListAsync();

        //    var productsDto = response
        //        .Select(prod => new ProductResponseDto
        //        {
        //            Id = prod.Id,
        //            Name = prod.Name,
        //            Description = prod.Description,
        //            SKU = prod.SKU,
        //            Stock = prod.Stock,
        //            Price = prod.Price,
        //            ImageUrl = prod.ImageUrl,
        //            CreatedAt = prod.CreatedAt,
        //            Categories = prod.Categories
        //            .Select(x => new CategoryResponseDto
        //            {
        //                Id = x.CategoryId,
        //                Name = x.Category?.Name
        //            }).ToList()
        //        });
        //    return Ok(productsDto);
        //}


        //[HttpGet("filtros")]
        //public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get([FromQuery] PaginationDto paginationDto, [FromQuery] ProductFiltroDto productFiltroDto)
        //{
        //    var queryable = _context.Products
        //        .AsNoTracking()
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(productFiltroDto.Name))
        //    {
        //        queryable = queryable.Where(x => x.Name.Contains(productFiltroDto.Name));
        //    }

        //    if (productFiltroDto.StockMenorACinco.HasValue)
        //    {
        //        if (productFiltroDto.StockMenorACinco.Value)
        //        {
        //            queryable = queryable.Where(x => x.Stock < 5);
        //        }
        //    }

        //    if (productFiltroDto.TieneImagen.HasValue)
        //    {
        //        if (productFiltroDto.TieneImagen.Value)
        //        {
        //            queryable = queryable.Where(x => x.ImageUrl != null);
        //        }
        //        else
        //        {
        //            queryable = queryable.Where(x => x.ImageUrl == null);
        //        }

        //    }

        //    if(productFiltroDto.PrecioMinimo.HasValue 
        //        && productFiltroDto.PrecioMaximo.HasValue)
        //    {
        //        var priceMin= productFiltroDto.PrecioMinimo.Value;
        //        var priceMax = productFiltroDto.PrecioMaximo.Value;
        //        queryable = queryable
        //            .Where(x => x.Price >= priceMin && x.Price <= priceMax);
        //    }
        //    if (!string.IsNullOrEmpty(productFiltroDto.NombreCategoria))
        //    {
        //        queryable = queryable
        //            .Where(x => x.Categories
        //            .Any(x => 
        //            x.Category!.Name == productFiltroDto.NombreCategoria));
        //    }


        //    await _contextAccessor.HttpContext.InsertarPaginacionHeader(queryable);

        //    var response = await queryable
        //        .Include(x => x.Categories)
        //        .ThenInclude(x => x.Category)
        //        .OrderBy(x => x.Id)
        //        .Paginate(paginationDto)
        //        .ToListAsync();

        //    var productsDto = response
        //        .Select(prod => new ProductResponseDto
        //        {
        //            Id = prod.Id,
        //            Name = prod.Name,
        //            Description = prod.Description,
        //            SKU = prod.SKU,
        //            Stock = prod.Stock,
        //            Price = prod.Price,
        //            ImageUrl = prod.ImageUrl,
        //            CreatedAt = prod.CreatedAt,
        //            Categories = prod.Categories
        //            .Select(x => new CategoryResponseDto
        //            {
        //                Id = x.CategoryId,
        //                Name = x.Category?.Name
        //            }).ToList()
        //        });
        //    return Ok(productsDto);
        //}

        //[HttpGet("otro-get")]
        //public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get
        //    ([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 1, [FromQuery] string nameFilterProduct = "")
        //{
        //    var plantillaQuery = _context.Products
        //        .Include(x => x.Categories)
        //        .ThenInclude(x => x.Category)
        //        .Where(x => x.IsActive)
        //        .Where(x => x.Name.Contains(nameFilterProduct))
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize);

        //    var ejecutandoConsultaYObteniendoDatos = await plantillaQuery.ToListAsync();

        //    var responseDto = ejecutandoConsultaYObteniendoDatos.Select(prod => new ProductResponseDto
        //    {
        //        Id = prod.Id,
        //        Name = prod.Name,
        //        Description = prod.Description,
        //        SKU = prod.SKU,
        //        Stock = prod.Stock,
        //        Price = prod.Price,
        //        ImageUrl = prod.ImageUrl,
        //        CreatedAt = prod.CreatedAt,
        //        Categories = prod.Categories
        //            .Select(x => new CategoryResponseDto
        //            {
        //                Id = x.CategoryId,
        //                Name = x.Category?.Name
        //            }).ToList()
        //    });

        //    return Ok(responseDto);

        //}


        [HttpGet("{id:int}", Name = "GetProductById")]
        public async Task<ActionResult<ProductResponseDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAync(id);
            if (product is null)
            {
                return NotFound();
            }
            return Ok(product);

        }

        //[HttpPost]
        //public async Task<ActionResult> Create([FromForm] ProductCreateDto productCreateDto)
        //{
        //    if (productCreateDto.CategoryIds is null || productCreateDto.CategoryIds.Count == 0)
        //    {
        //        ModelState.AddModelError(nameof(productCreateDto.CategoryIds),
        //            "No se puede crear producto sin categoria");
        //        return ValidationProblem();
        //    }

        //    var categoriesDb = await _context.Categories
        //        .Where(c => productCreateDto.CategoryIds.Contains(c.Id))
        //        .Select(x => new { x.Id, x.Name }).ToListAsync();

        //    if (categoriesDb.Count != productCreateDto.CategoryIds.Count)
        //    {
        //        var categoriesExists = categoriesDb.Select(x => x.Id).ToList();

        //        var categoriesNoExists = productCreateDto.CategoryIds.Except(categoriesExists);
        //        var categoriesNoExistsString = string.Join(",", categoriesNoExists);
        //        ModelState.AddModelError(nameof(productCreateDto.CategoryIds),
        //            $"Los siguientes ids de categorias no existen:{categoriesNoExistsString}");
        //        return ValidationProblem();
        //    }



        //    var product = new Product
        //    {
        //        Name = productCreateDto.Name,
        //        Description = productCreateDto.Description,
        //        SKU = productCreateDto.SKU,
        //        // Redondeamos a 2 decimales usando el "redondeo comercial" (hacia el número más lejano de cero)
        //        Price = Math.Round(productCreateDto.Price, 2, MidpointRounding.AwayFromZero),
        //        //ImageUrl = productCreateDto.ImageUrl,
        //        Categories = productCreateDto.CategoryIds
        //        .Select(x => new CategoryProduct { CategoryId = x }).ToList(),
        //        Stock = productCreateDto.Stock,
        //    };

        //    if (productCreateDto.ImageUrl is not null)
        //    {
        //        var url = await _almacenadorArchivos.Almacenar(contenedor, productCreateDto.ImageUrl);
        //        product.ImageUrl = url;

        //    }
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    var productDto = new ProductResponseDto
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        SKU = product.SKU,
        //        Stock = product.Stock,
        //        Price = product.Price,
        //        ImageUrl = product.ImageUrl,
        //        CreatedAt = product.CreatedAt,
        //        Categories = categoriesDb
        //        .Select(x => new CategoryResponseDto
        //        {
        //            Id = x.Id,
        //            Name = x.Name
        //        }).ToList()
        //    };

        //    return CreatedAtRoute("GetProductById", new { id = product.Id }, productDto);
        //}

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDto productUpdateDto)
        //{
        //    var product = await _context.Products
        //        .Include(x => x.Categories)
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    if (product is null)
        //    {
        //        return NotFound();
        //    }

        //    if (productUpdateDto.CategoryIds is null || productUpdateDto.CategoryIds.Count == 0)
        //    {
        //        ModelState.AddModelError(nameof(productUpdateDto.CategoryIds),
        //            "No se puede crear producto sin categoria");
        //        return ValidationProblem();
        //    }

        //    var categoryIdsExists = await _context.Categories
        //        .Where(x => productUpdateDto.CategoryIds.Contains(x.Id))
        //        .Select(x => x.Id)
        //        .ToListAsync();

        //    if (categoryIdsExists.Count != productUpdateDto.CategoryIds.Count)
        //    {
        //        var categoriesNoExists = productUpdateDto.CategoryIds.Except(categoryIdsExists);
        //        var categoriesStringNoExists = string.Join(",", categoriesNoExists);
        //        ModelState.AddModelError(nameof(productUpdateDto.CategoryIds),
        //            $"Las siguientes categorias no existe:{categoriesStringNoExists}");
        //        return ValidationProblem();
        //    }

        //    if (productUpdateDto.ImageUrl is not null)
        //    {
        //        var url = await _almacenadorArchivos.RemplazarArchivo
        //            (product.ImageUrl, contenedor, productUpdateDto.ImageUrl);
        //        product.ImageUrl = url;
        //    }

        //    product.Name = productUpdateDto.Name;
        //    product.Description = productUpdateDto.Description;
        //    product.Price = productUpdateDto.Price;
        //    product.SKU = productUpdateDto.SKU;
        //    product.Stock = productUpdateDto.Stock;
        //    product.Categories = productUpdateDto.CategoryIds
        //        .Select(x => new CategoryProduct { CategoryId = x })
        //        .ToList();

        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        //[HttpPatch("{id:int}")]
        //public async Task<IActionResult> Patch(int id, JsonPatchDocument<ProductPatchDto> patchDoc)
        //{
        //    var product = await _context.Products
        //        .Include(x => x.Categories)
        //        .FirstOrDefaultAsync(p => p.Id == id);

        //    if (product is null)
        //    {
        //        return NotFound();
        //    }

        //    var productPatchDto = new ProductPatchDto()
        //    {
        //        Name = product.Name,
        //        Description = product.Description,
        //        SKU = product.SKU,
        //        Stock = product.Stock,
        //        CategoryIds = product.Categories.Select(x => x.CategoryId).ToList(),
        //        ImageUrl = product.ImageUrl,
        //        Price = product.Price
        //    };
        //    //aplicando los cambios que vienen del doc patch del cliente a los campos
        //    //respectivos del dto
        //    patchDoc.ApplyTo(productPatchDto, ModelState);

        //    var isValidProduct = TryValidateModel(productPatchDto);

        //    if (!isValidProduct)
        //    {
        //        return ValidationProblem();
        //    }
        //    // Verificando si las categorias existen en la DB
        //    if (productPatchDto.CategoryIds is null || productPatchDto.CategoryIds.Count == 0)
        //    {
        //        ModelState.AddModelError(nameof(productPatchDto.CategoryIds),
        //            "No se puede actualizar producto sin categoria");
        //        return ValidationProblem();
        //    }

        //    var categoryIdsExists = await _context.Categories
        //        .Where(x => productPatchDto.CategoryIds.Contains(x.Id))
        //        .Select(x => x.Id)
        //        .ToListAsync();

        //    if (categoryIdsExists.Count != productPatchDto.CategoryIds.Count)
        //    {
        //        var categoriesNoExists = productPatchDto.CategoryIds.Except(categoryIdsExists);
        //        var categoriesStringNoExists = string.Join(",", categoriesNoExists);
        //        ModelState.AddModelError(nameof(productPatchDto.CategoryIds),
        //            $"Las siguientes categorias no existe:{categoriesStringNoExists}");
        //        return ValidationProblem();
        //    }

        //    //actualizando el state de la entidad producto traida de la db

        //    product.Name = productPatchDto.Name;
        //    product.Description = productPatchDto.Description;
        //    product.Price = productPatchDto.Price;
        //    product.ImageUrl = productPatchDto.ImageUrl;
        //    product.SKU = productPatchDto.SKU;
        //    product.Stock = productPatchDto.Stock;
        //    product.Categories = productPatchDto.CategoryIds
        //        .Select(x => new CategoryProduct { CategoryId = x }).ToList();

        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpDelete("{id:int}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        //    if (product is null)
        //    {
        //        return NotFound();
        //    }
        //    product.IsActive = false;

        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

    }
}
