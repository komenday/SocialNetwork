using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using SQRS.Core.Infrastructure;

namespace Post.Query.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PostLookupController : ControllerBase
{
    private readonly ILogger<PostLookupController> _logger;
    private readonly IQueryDispatcher<PostEntity> _queryDispatcher;

    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindAllPostsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex, "Error while processing request to recieve all posts");
        }
    }

    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetPostByIdAsync(Guid postId)
    {
        try
        {
            var post = await _queryDispatcher.SendAsync(new FindPostByIdQuery { Id = postId });
            return NormalResponse(post);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex, "Error while processing request to find a post by ID");
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex, $"Error while processing request to find posts from {author}");
        }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsWithCommentsAsync()
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex, $"Error while processing request to find posts with comments");
        }
    }

    [HttpGet("withLikes/{numOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int numOfLikes)
    {
        try
        {
            var posts = await _queryDispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = numOfLikes });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            return ErrorResponse(ex, $"Error while processing request to find posts with {numOfLikes} or more likes");
        }
    }

    private ActionResult NormalResponse(List<PostEntity>? posts)
    {
        if (posts is null || !posts.Any())
        {
            return NoContent();
        }

        var count = posts.Count;

        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} posts{(count > 1 ? "s" : string.Empty)}"
        });
    }

    private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
    {
        _logger.LogError(ex, safeErrorMessage);

        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage
        });
    }
}
