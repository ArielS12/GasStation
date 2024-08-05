using GasStation.Infrastucture.Constants;
using GasStation.Model;
using System.Diagnostics;

namespace GasStation.Main;

public  class PumpClient(IEnumerable<Pump> pump)
{
    private Stopwatch _watcher = new();
    private IEnumerable<Pump> _pumps = pump;

    /// <summary>
    /// Comienza la carga de un vehiculo, el estado del surtidor debe ser Libre
    /// Se debe indicar el número del surtidor
    /// </summary>
    /// <param name="pumpId"></param>
    public void StartCharging(int pumpId)
    {
        var _pump = _pumps.FirstOrDefault(x => x.Id.Equals(pumpId));
        _pump.Status = PUMP_STATUS.CHARGING;
        _watcher.Start();
        if (_pump.PredefinedMaxAmount != null)
        {
            var milliseconds = _pump.PredefinedMaxAmount / LibraryConstants.FUEL_PRICE_PER_MILISECOND;
            System.Timers.Timer runonce = new(milliseconds.Value);
            runonce.Elapsed += (s, e) =>
            {
              _pump.Amount = StopCharging(pumpId);
            };
            runonce.AutoReset = false;
            runonce.Start();
        }
    }

    /// <summary>
    /// Detiene la carga del vehiculo, el estado debe ser Cargando
    /// La carga se debe detener si no fue indicado un monto predefinido, en caso contrario se dentra automaticamente
    /// Se debe indicar el número del surtidor
    /// </summary>
    /// <param name="pumpId"></param>
    /// <returns></returns>

    public double StopCharging(int pumpId)
    {
        var _pump = _pumps.FirstOrDefault(x => x.Id.Equals(pumpId));
        var response = _watcher.ElapsedMilliseconds * LibraryConstants.FUEL_PRICE_PER_MILISECOND;
        _watcher.Stop();
        return _pump.PredefinedMaxAmount != null ? (response > _pump.PredefinedMaxAmount ? _pump.PredefinedMaxAmount.Value : response) : response;
    }
}
