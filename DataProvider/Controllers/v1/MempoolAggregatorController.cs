using Microsoft.AspNetCore.Mvc;

namespace DataProvider.Controllers.v1;


[Route("api/v1/[controller]/[action]")]
[ApiController]
public class MempoolAggregatorController: ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTotalCount()
    {
        return Ok("ok");
    }
}