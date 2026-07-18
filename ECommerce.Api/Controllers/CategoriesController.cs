using ECommerce.Api.Data;
using ECommerce.Api.DTOs.Categories;
using ECommerce.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> Get()
        {
            var categories=await _context.Categories.Where(x=>x.IsActive).ToListAsync();
            var categoriesResponseDto = categories
                .Select(cat=>new CategoryResponseDto
                {
                    Id=cat.Id,
                    Name=cat.Name
                });
            return Ok(categoriesResponseDto);
        }

        [HttpGet("{id:int}",Name ="GetCategoryById")]
        public async Task<ActionResult<CategoryResponseDto>> Get(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(cat=>cat.Id==id);
            if(category is null)
            {
                return NotFound();
            }

            var categoryResponseDto = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
            return Ok(categoryResponseDto);
        }



        [HttpPost]
        public async Task<IActionResult> Post(CategoryCreateDto categoryCreateDto)
        {
            var category = new Category
            {
                Name = categoryCreateDto.Name,
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok();
        }




    }
}
