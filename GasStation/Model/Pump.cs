using GasStation.Infrastucture.Constants;
using GasStation.Interface;
using System.Diagnostics;

namespace GasStation.Model;

public class Pump : IPump
{
    public int Id { get; set; }
    public PUMP_STATUS Status { get; set; } = PUMP_STATUS.BLOCKED;
    public double? PredefinedMaxAmount { get; set; }
    public double Amount { get; set; }

    private Stopwatch _watcher = new();

    /// <summary>
    /// Comienza la carga de un vehiculo, el estado del surtidor debe ser Libre
    /// Se debe indicar el número del surtidor
    /// </summary>
    /// <param name="pumpId"></param>
    public void StartCharging(int pumpId)
    {
        Status = PUMP_STATUS.CHARGING;
        _watcher.Start();
        if (PredefinedMaxAmount != null)
        {
            var milliseconds = PredefinedMaxAmount / LibraryConstants.FUEL_PRICE_PER_MILISECOND;
            System.Timers.Timer runonce = new(milliseconds.Value);
            runonce.Elapsed += (s, e) =>
            {
                Amount = StopCharging(pumpId);
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
    /// <returns>El valor en euros que se cargo en combustible</returns>

    public double StopCharging(int pumpId)
    {
        var response = _watcher.ElapsedMilliseconds * LibraryConstants.FUEL_PRICE_PER_MILISECOND;
        _watcher.Stop();
        return PredefinedMaxAmount != null ? (response > PredefinedMaxAmount ? PredefinedMaxAmount.Value : response) : response;
    }
}
