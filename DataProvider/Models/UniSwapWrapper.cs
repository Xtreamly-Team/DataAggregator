using InfluxDB.Client.Core;

namespace DataProvider.Models;

[Measurement("UniSwapSwap")]
public  class UniSwapWrapper
{
    [Column("TxHash", IsTag = true)]
    public  Swap Swap { get; init; }

    [Column("executedPrice")]
    public decimal ExecutedPrice => Math.Abs(decimal.Parse(Swap.Amount1) / decimal.Parse(Swap.Amount0));

    public bool IsBuy =>  decimal.Parse(Swap.Amount0)  > 0;
}