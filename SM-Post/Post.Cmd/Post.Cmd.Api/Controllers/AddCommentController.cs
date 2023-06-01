using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;
using SQRS.Core.Exceptions;
using SQRS.Core.Infrastructure;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AddCommentController : ControllerBase
{
    private readonly ILogger<AddCommentController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public AddCommentController(ILogger<AddCommentController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> AddCommentAsync(Guid id, AddCommentCommand command)
    {
        try
        {
            command.Id = id;
            await _commandDispatcher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Add comment request is completed successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Client made a bad request");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (AggregateNotFoundException ex)
        {
            _logger.Log(LogLevel.Warning, ex, "Could not retrieve an aggregate, client passed an incorrect post ID");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            const string SafeErrorMessage = "Error while processing request to add a comment to a post the post";
            _logger.Log(LogLevel.Error, ex, SafeErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = SafeErrorMessage
            });
        }
    }
}
