using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
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
            try
            {
                var categories = await context.Categories.OrderBy(x => x.Id).ToListAsync();
                return Ok(categories);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Impossible to create the category.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category is null)
                    return NotFound(new { message = $"The category of ID {id} was not found." });

                return Ok(category);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Impossible to get the category.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error.");
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync([FromServices] BlogDataContext context, [FromBody] EditorCategoryViewModel viewModel)
        {
            try
            {
                var category = new Category(id: 0, viewModel.Name, viewModel.Slug.ToLower());

                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return CreatedAtAction(actionName: nameof(GetByIdAsync),
                                       routeValues: new { id = category.Id },
                                       value: category);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Impossible to create the category.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error.");
            }
        }

        [HttpPut("update/{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromServices] BlogDataContext context, [FromRoute] int id, [FromBody] EditorCategoryViewModel viewModel)
        {
            try
            {
                var currentCategory = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (currentCategory is null)
                    return NotFound(new { message = $"The category of Id {id} was not found. Make sure you pass the correct ID in route parameter." });

                currentCategory.Name = viewModel.Name;
                currentCategory.Slug = viewModel.Slug;

                context.Categories.Update(currentCategory);

                await context.SaveChangesAsync();

                return CreatedAtAction(actionName: nameof(GetByIdAsync),
                                       routeValues: new { id = currentCategory.Id },
                                       value: currentCategory);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Impossible to update the category.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error.");
            }
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category is null)
                    return NotFound(new { message = $"The category of Id {id} was not found. Make sure you pass the correct ID in route parameter." });

                context.Categories.Remove(category);

                await context.SaveChangesAsync();

                return Ok(new { message = "The category was successfully deleted." });
            }
            catch (DbUpdateException)
            {
                return BadRequest("Impossible to delete the category.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal error.");
            }
        }
    }
}