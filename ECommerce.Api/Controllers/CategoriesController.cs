using ECommerce.Api.Data;
using ECommerce.Api.DTOs;
using ECommerce.Api.DTOs.Categories;
using ECommerce.Api.Entities;
using ECommerce.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController:ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        
        public CategoriesController(ApplicationDbContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _contextAccessor = httpContextAccessor;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> Get([FromQuery]PaginationDto paginationDto)
        {
            var queryable = _context.Categories.Where(x => x.IsActive).AsQueryable();

            await _contextAccessor.HttpContext.InsertarPaginacionHeader(queryable);

            var response = await queryable
                .OrderBy(x => x.Id)
                .Paginate(paginationDto)
                .ToListAsync();

            //var categories=await _context.Categories.Where(x=>x.IsActive).ToListAsync();
            var categoriesResponseDto = response
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
