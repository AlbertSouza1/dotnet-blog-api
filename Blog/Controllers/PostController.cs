using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetPostsAsync([FromServices] BlogDataContext context, [FromQuery] int page = 0, [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();

                var posts = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(statusCode: 500, value: new ResultViewModel<ListPostsViewModel>(error: "Internal error."));
            }
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> GetPostByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var post = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Category)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .FirstOrDefaultAsync(x => x.Id == id);

                return Ok(new ResultViewModel<ListPostsViewModel>(post));
            }
            catch
            {
                return StatusCode(statusCode: 500, value: new ResultViewModel<ListPostsViewModel>(error: "Internal error."));
            }
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetPostsByCategoryAsync([FromServices] BlogDataContext context,
                                                                 [FromRoute] string category,
                                                                 [FromQuery] int page = 0,
                                                                 [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();

                var posts = await context.Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .Where(x => x.Category.Slug == category)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .OrderByDescending(x => x.LastUpdateDate)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(statusCode: 500, value: new ResultViewModel<ListPostsViewModel>(error: "Internal error."));
            }
        }
    }
}