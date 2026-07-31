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

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(int id, JsonPatchDocument<ProductPatchDto> patchDoc)
        {
            if (patchDoc is null)
            {
                return BadRequest();
            }

            var productPathDto = await _productService.GetPatchDtoForUpdate(id);

            if (productPathDto is null)
            {
                return NotFound(new { message = $"El producto con id {id} no existe." });
            }

            //aplicar los cambios del json doc al dto

            patchDoc.ApplyTo(productPathDto, ModelState);

            var isValidDto = TryValidateModel(productPathDto);
            if (!isValidDto)
            {

                return ValidationProblem(ModelState);
            }

            try
            {
                // presistiendo los cambios en la DB
                var productDb = await _productService.UpdatePatchAsync(id, productPathDto);
                if (!productDb)
                {
                    return NotFound(new { message = $"El producto con id {id} no existe." });
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                var key = ex.ParamName ?? string.Empty;
                ModelState.AddModelError(key, ex.Message);
                return ValidationProblem(ModelState);
            }

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var productDb = await _productService.DeleteLogicAsync(id);
            if (!productDb)
            {
                return NotFound(new { message = $"El producto con id {id} no existe" });
            }
            return NoContent();


        }

    }
}
