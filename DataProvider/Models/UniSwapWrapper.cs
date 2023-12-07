namespace DataProvider.Models;

public readonly struct UniSwapWrapper
{

    public required Swap Swap { get; init; }

    public decimal ExecutedPrice => Math.Abs(decimal.Parse(Swap.Amount1) / decimal.Parse(Swap.Amount0));
}