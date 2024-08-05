using GasStation.Model;

namespace GasStation.Main;

public interface IStationClient
{
    PumpActionResult DoPumpAction(int id, PUMP_ACTION newState);
    IEnumerable<Supply> GetLog();
    IEnumerable<PUMP_STATUS> GetPumpsStates();
    PumpActionResult SetPredefinedAmount(int id, double amount);
}