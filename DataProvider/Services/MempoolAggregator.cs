using DataProvider.Models;
using Mediator;
using Nethereum.Web3;
using XtreamlyAggregatorSDK;

namespace DataProvider.Services;

public class MempoolAggregator : IHostedService
{
    
    private Thread _worker;
    private bool _shouldRun = false;
    private readonly IMediator _mediator;
    private readonly ILogger<UniSwapAggregator> _logger;
    private readonly XtreamlyAggregator<MemPoolData> _aggregator;

    public MempoolAggregator(IMediator mediator, ILogger<UniSwapAggregator> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _aggregator = new XtreamlyAggregator<MemPoolData>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _shouldRun = true;
        _worker = new Thread(runner)
        {
            Priority = ThreadPriority.Highest
        };
        _worker.Start();
        return Task.CompletedTask;
    }

    private async void runner()
    {
        _logger.LogInformation("Mempool service running...");
        var web3 = new Web3(Environment.GetEnvironmentVariable("rpc"));
        var pendingFilter = await web3.Eth.Filters.NewPendingTransactionFilter.SendRequestAsync();

        var filterChanges = new List<string>();
        while (_shouldRun)
        {
            try
            {
                try
                {
                    filterChanges =
                        (await web3.Eth.Filters.GetFilterChangesForBlockOrTransaction.SendRequestAsync(pendingFilter)).ToList();
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e.Message);
                    pendingFilter = await web3.Eth.Filters.NewPendingTransactionFilter.SendRequestAsync();
                    Thread.Sleep(1000);
                    continue;
                }

                Parallel.ForEach(filterChanges, value =>
                {
                    _aggregator.Send(new MemPoolData
                    {
                        TxHash = value,
                        SessionCount = 0
                    });
                });
                Thread.Sleep(30);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _shouldRun = false;
        _worker.Join();
        return Task.CompletedTask;
    }
}