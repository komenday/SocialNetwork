using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;
using SQRS.Core.Infrastructure;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class RestoreReadDbController : ControllerBase
{
    private readonly ILogger<RestoreReadDbController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> RestoreReadDbAsync()
    {

        try
        {
            await _commandDispatcher.SendAsync(new RestoreReadDbCommand());

            return StatusCode(StatusCodes.Status201Created, new BaseResponse
            {
                Message = "Read database restore request was completed successfully"
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
        catch (Exception ex)
        {
            const string SafeErrorMessage = "Error while processing request to restore read database";
            _logger.Log(LogLevel.Error, ex, SafeErrorMessage);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = SafeErrorMessage
            });
        }
    }
}
