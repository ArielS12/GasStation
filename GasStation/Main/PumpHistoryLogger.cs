

using GasStation.Model;

namespace GasStation.Main;

internal class PumpHistoryLogger
{
    private IList<Supply> _supplies = [];

    public void AddLog(Supply supply) => _supplies.Add(supply);

    public IEnumerable<Supply> GetLog() => _supplies.OrderByDescending(s => s.Amount).ThenByDescending(s => s.CreatedDate);
}
