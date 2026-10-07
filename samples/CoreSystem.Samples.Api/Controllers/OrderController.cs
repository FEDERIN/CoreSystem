using CoreSystem.Samples.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreSystem.Samples.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IProductService myService) : ControllerBase
{

    [HttpGet("data/{id}")]
    public async Task<IActionResult> GetData(string id)
    {
        var result = await myService.GetDataAsync(id);
        return Ok(result);
    }

    [HttpPost("data")]
    public IActionResult PostData([FromBody] object data)
    {
        return Ok(data);
    }
}