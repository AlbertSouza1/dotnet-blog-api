using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet("")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            var categories = await context.Categories.OrderBy(x => x.Id).ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
                return NotFound(new { message = $"The category of ID {id} was not found." });

            return Ok(category);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromServices] BlogDataContext context, [FromBody] Category category)
        {
            if (!category.IsValid())
                return BadRequest(new { message = "Failed to convert the body Json to a Category. Make sure the given Json is in the corret format." });

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return CreatedAtAction(actionName: nameof(GetByIdAsync),
                                   routeValues: new { id = category.Id },
                                   value: category);
        }

        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromServices] BlogDataContext context, [FromRoute] int id, [FromBody] Category category)
        {
            if (!category.IsValid())
                return BadRequest(new { message = $"Failed to convert the body Json to a Category. Make sure the given Json is in the corret format." });

            var currentCategory = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (currentCategory is null)
                return NotFound(new { message = $"The category of Id {id} was not found. Make sure you pass the correct ID in route parameter." });

            currentCategory.Name = category.Name;
            currentCategory.Slug = category.Slug;

            context.Categories.Update(currentCategory);

            await context.SaveChangesAsync();

            return CreatedAtAction(actionName: nameof(GetByIdAsync),
                                   routeValues: new { id = currentCategory.Id },
                                   value: currentCategory);
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
                return NotFound(new { message = $"The category of Id {id} was not found. Make sure you pass the correct ID in route parameter." });

            context.Categories.Remove(category);

            await context.SaveChangesAsync();

            return Ok(new { message = "The category was successfully deleted." });
        }
    }
}