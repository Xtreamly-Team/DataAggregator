using DataProvider.Models;
using DataProvider.Services.Queries;
using Mediator;

namespace DataProvider.Services;

public class UniSwapAggregator : IHostedService
{
    private int _count = 0;
    private Thread _worker;
    private bool _shouldRun = false;
    private readonly IMediator _mediator;
    private readonly ILogger<UniSwapAggregator> _logger;

    public UniSwapAggregator(IMediator mediator, ILogger<UniSwapAggregator> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        System.IO.Directory.CreateDirectory("/uniswap/");
        _worker = new Thread(runner)
        {
            Priority = ThreadPriority.Highest
        };
        _shouldRun = true;
        _worker.Start();
    }


    private async void runner()
    {
        while (_shouldRun)
        {
            try
            {
                var swaps = await  _mediator.Send(new UniSwapRawQuery(50));
                swaps.EnsureSuccess();
                Parallel.ForEach(swaps.ActualValue, swap =>
                {
                    var wrapper = new UniSwapWrapper()
                    {
                        Swap = swap
                    };
                    if (System.IO.File.Exists($"/uniswap/{swap.Transaction.TxId}")) return;
                    System.IO.File.WriteAllText($"/uniswap/{swap.Transaction.TxId}", System.Text.Json.JsonSerializer.Serialize(wrapper));
                    _logger.LogInformation($"uniswap swap was logged {swap.Transaction.TxId}");
                    Interlocked.Increment(ref _count);
                });
                _logger.LogInformation("total swaps captured in current session : {Count}", _count);
                _logger.LogInformation("total swaps captured ever : {Count}", System.IO.Directory.GetFiles("/uniswap/").Length);
                _logger.LogInformation("sleeping for 1 sec");
                System.Threading.Thread.Sleep(12000);
            }
            catch (Exception e)
            {
               _logger.LogWarning("uniswap fetching is failing : {ERROR}", e.Message);
            }
           
            
        }
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}