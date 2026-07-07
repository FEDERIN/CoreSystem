using Core.Cache.Attributes;
using CoreSystem.Samples.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreSystem.Samples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IMyService myService) : ControllerBase
{

    [HttpGet("data/{id}")]
    [Cacheable(tag: "Order", expirationSeconds: 500)]
    public async Task<IActionResult> GetData(string id)
    {
        var result = await myService.GetDataAsync(id);
        return Ok(result);
    }

    [HttpPost("data")]
    public async Task<IActionResult> PostData([FromBody] object data)
    {
        return Ok(new JsonResult(data));
    }
}