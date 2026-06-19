using CoreSystem.Samples.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreSystem.Samples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IMyService myService) : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello() => Ok(new { message = "Tracing is active!" });

    [HttpPost("process-order")]
    public IActionResult ProcessOrder()
    {
        return Ok(new
        {
            id = Guid.NewGuid(),
            status = "Processed",
            processedAt = DateTime.UtcNow
        });
    }

    [HttpDelete("delete-item/{id}")]
    public IActionResult DeleteItem(Guid id)
    {
        return NoContent();
    }

    [HttpGet("data/{id}")]
    public async Task<IActionResult> GetData(string id)
    {
        var result = await myService.GetDataAsync(id);
        return Ok(result);
    }
}