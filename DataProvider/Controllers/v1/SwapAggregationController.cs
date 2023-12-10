using System.Diagnostics;
using DataProvider.Services.Queries;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace DataProvider.Controllers.v1;


[ApiController]
[Route("/api/v1/[controller]/[action]")]
public class SwapAggregationController : ControllerBase
{
    private readonly IMediator _mediator;

    public SwapAggregationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpGet]
    public async Task<IActionResult> UniSwapRaw([FromQuery] int count = 10)
    {
        try
        {
            var timer = new Stopwatch();
            timer.Start();
            
            var result = await _mediator.Send(new UniSwapRawQuery(count));
            
            timer.Stop();
            if (!result.IsSuccessful)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result.Error.Message);
            }
            return Ok(new
            {
                result = result.ActualValue,
                time = timer.ElapsedMilliseconds + " ms"
            });


        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
    
    [HttpGet]
    public async Task<IActionResult> UniSwapSaved([FromQuery] string txId )
    {
        try
        {
            var timer = new Stopwatch();
            timer.Start();

            if (!System.IO.File.Exists("/uniswap/" + txId))
            {
                return NotFound("transaction was not found");
            }

            var result = System.IO.File.ReadAllText("/uniswap/" + txId);

            return Ok(new
            {
                result = result,
                time = timer.ElapsedMilliseconds + " ms"
            });


        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    
}