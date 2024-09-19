namespace DotNet8.ExceptionHandlerMiddleware.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateBlog(BlogModel blog)
    {
        try
        {
            int blogTitle=Convert.ToInt32(blog.BlogTitle);
            return Ok(blog);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
