using System.Text;
using DataProvider.Models;
using DataProvider.Services.Queries;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Mediator;
using XtreamlyAggregatorSDK;

namespace DataProvider.Services;

public class UniSwapAggregator : IHostedService
{
    private int _count = 0;
    private Thread _worker;
    private bool _shouldRun = false;
    private readonly IMediator _mediator;
    private readonly ILogger<UniSwapAggregator> _logger;
    private readonly XtreamlyAggregator<UniSwapWrapper> _aggregator;

    public UniSwapAggregator(IMediator mediator, ILogger<UniSwapAggregator> logger)
    {
        _mediator = mediator;
        _logger = logger;
        _aggregator = new XtreamlyAggregator<UniSwapWrapper>();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _shouldRun = true;
        _worker = new Thread(runner)
        {
            Priority = ThreadPriority.Highest
        };
        _worker.Start();
    }


    private async void runner()
    {
        _logger.LogInformation("Uniswap swap service running...");
        while (_shouldRun)
        {
            try
            {
                var swaps = await  _mediator.Send(new UniSwapRawQuery(50));
                swaps.EnsureSuccess();
                Parallel.ForEach(swaps.ActualValue, async swap =>
                {
                    _aggregator.WriteApi.WriteRecord(ConvertToInfluxDBLineProtocol(new UniSwapWrapper()
                    {
                        Swap = swap
                    }), WritePrecision.Ms, "UniSwapWrapper", "xtreamly" );
                });
               
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
        _shouldRun = false;
        _worker.Join();
        return Task.CompletedTask;
    }
    
    public string ConvertToInfluxDBLineProtocol(UniSwapWrapper data)
    {
        const string measurement = "swap";
        var tags = $"token0={data.Swap.Token0.Symbol},token1={data.Swap.Token1.Symbol},isBuy={data.IsBuy}";
        var fields = $"amount0={data.Swap.Amount0},amount1={data.Swap.Amount1},amountUSD={data.Swap.AmountUsd},tick={data.Swap.Tick},gasPrice={data.Swap.Transaction.GasPrice},gasUsed={data.Swap.Transaction.GasUsed},executedPrice={data.ExecutedPrice},SqrtPriceX96={data.Swap.SqrtPriceX96}";
        var result = ($"{measurement},{tags} {fields} {data.Swap.Timestamp}");
        return $"swap,{tags} {fields}";
        return result;
    }
}