using ECommerce.Api.DTOs;
using ECommerce.Api.DTOs.Products;
using ECommerce.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get([FromQuery] ProductFiltroDto productFiltroDto, [FromQuery] PaginationDto paginationDto)
        {
            var products = await _productService.GetAllAsync(productFiltroDto, paginationDto);
            return products.ToList();
        }

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

        [HttpPost]
        public async Task<ActionResult> Create([FromForm] ProductCreateDto productCreateDto)
        {
            try
            {
                var productDto = await _productService.CreateAsync(productCreateDto);
                return CreatedAtRoute("GetProductById", new { id = productDto.Id }, productDto);
            }
            catch (ArgumentException ex)
            {
                var key = ex.ParamName ?? string.Empty;
                ModelState.AddModelError(key, ex.Message);
                return ValidationProblem();
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductUpdateDto productUpdateDto)
        {
            try
            {
                var product = await _productService.UpdateAsync(id, productUpdateDto);

                if (!product)
                {
                    return NotFound(new { message = $"No se encontró el producto con ID: {id}" });
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                var key = ex.ParamName ?? string.Empty;
                ModelState.AddModelError(key, ex.Message);
                return ValidationProblem();
            }
        }

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
