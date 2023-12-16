using InfluxDB.Client.Core;

namespace DataProvider.Models;

[Measurement("MemPoolData")]
public class MemPoolData
{
    [Column("TxHash", IsTag = true)] public string TxHash { get; set; }

    [Column("SessionCount")] public long SessionCount { get; set; }
}