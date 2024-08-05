

using GasStation.Model;

namespace GasStation.Main;

/// <summary>
/// Desde este cliente se puede manejar todas las acciones que realiza un surtidor
/// Se le debe indicar la cantidad de surtidores que posee la pista y le asigna 
/// un número a cada uno.
/// Si se integra en una API es necesario declararlo como singleton
/// </summary>
/// <param name="pumps"></param>
public class StationClient(IEnumerable<Pump> pumps) : IStationClient
{
    private IEnumerable<Pump> _pumps = pumps;
    private readonly PumpHistoryLogger _logger = new();

    /// <summary>
    /// Realiza las acciones que modifican el estado de un surtidor
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <param name="newState">nuevo estado del surtidor</param>
    /// <returns>Si la accion se pudo realizar correctamente o no</returns>
    public PumpActionResult DoPumpAction(int id, PUMP_ACTION newState)
    {
        var result = new PumpActionResult();
        switch (newState)
        {
            case PUMP_ACTION.RELEASE:
                result = SetAsFree(id);
                break;
            case PUMP_ACTION.BLOCK:
                result = SetAsBlocked(id);
                break;
            case PUMP_ACTION.START_CHARGING:
                result = StartCharging(id);
                break;
            case PUMP_ACTION.STOP_CHARGING:
                result = StopCharging(id);
                break;
        }
        return result;
    }

    /// <summary>
    /// Cambie el estado del surtidor de Cargando a Bloqueado y
    /// crea un log con los datos de la ultima carga
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <returns>Si la accion se pudo realizar correctamente o no</returns>
    private PumpActionResult StopCharging(int id) 
    {
        var result = new PumpActionResult();

        var pump = GetPump(id);
        if (pump.Status != PUMP_STATUS.CHARGING)
        {
            result.Success = false;
            result.ErrorMessage = $"El surtidor debe estar siendo utilizado. el estado actual es: {pump.Status}";
        }
        else
        {
            pump.Amount = pump.StopCharging();
            AddToLog(pump);
            SetAsBlocked(id);           
        }
        return result;
    }

    /// <summary>
    /// Cambia el estado del surtidor de Libre a Cargando
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <returns>Si la accion se pudo realizar correctamente o no</returns>
    private PumpActionResult StartCharging(int id)
    {
        var result = new PumpActionResult();
        var pump = GetPump(id);

        if (pump.Status != PUMP_STATUS.FREE)
        {
            result.Success = false;
            result.ErrorMessage = $"El surtidor debe estar liberado. el estado actual es: {pump.Status}";
        }
        else
        {
            pump.StartCharging();
        }
        return result;

    }


    /// <summary>
    /// Crea un nuevo registro de una carga
    /// </summary>
    /// <param name="pump">Datos del surtidor</param>
    private void AddToLog(Pump pump) {
        var supply = new Supply
        {
            PumpId = pump.Id,
            PredefinedAmount = pump.Amount,
            Amount = pump.Amount,
            CreatedDate = DateTime.UtcNow
        };
        _logger.AddLog(supply);
    }


    /// <summary>
    /// Cambia el estado de bloqueado a Libre
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <returns>Si la accion se pudo realizar correctamente o no</returns>
    private PumpActionResult SetAsFree(int id)
    {
        var result = new PumpActionResult();
        var pump = GetPump(id);

        if (pump.Status == PUMP_STATUS.FREE || pump.Status == PUMP_STATUS.CHARGING)
        {
            result.Success = false;
            result.ErrorMessage = $"El surtidor debe estar bloqueado. el estado actual es: {pump.Status}";
        }
        else
        {
            pump.Status = PUMP_STATUS.FREE;
        }
        return result;
    }


    /// <summary>
    /// Agrega un monton predefinido al surtidor, para hacer esta accion
    /// el estado del surtidor debe ser Bloqueado
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <param name="amount">Monto en euros que se van a cargar</param>
    /// <returns>Si la accion se pudo realizar correctamente o no</returns>
    public PumpActionResult SetPredefinedAmount(int id, double amount)
    {
        var result = new PumpActionResult();
        var pump = GetPump(id);
        if (pump.Status == PUMP_STATUS.FREE || pump.Status == PUMP_STATUS.CHARGING)
        {
            result.Success = false;
            result.ErrorMessage = $"El surtidor debe estar bloqueado. el estado actual es: {pump.Status}";
        }
        else
        {
            pump.PredefinedMaxAmount = amount;
        }

        return result;
    }

    /// <summary>
    /// Cambia el estado del surtidor a Bloqueado
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <returns>Si la accion se pudo realizar correctamente o no</returns>
    private PumpActionResult SetAsBlocked(int id)
    {
        var result = new PumpActionResult();

        var pump = GetPump(id);
        pump.Status = PUMP_STATUS.BLOCKED;
        pump.PredefinedMaxAmount = null;

        return result;

    }


    /// <summary>
    ///
    /// </summary>
    /// <returns>La lista de los registros de carga</returns>
    public IEnumerable<Supply> GetLog() => _logger.GetLog();

    /// <summary>
    /// Devuelve la información de un surtidor
    /// </summary>
    /// <param name="id">Número de surtidor</param>
    /// <returns>La informacion de un surtidor</returns>
    /// <exception cref="Exception"></exception>
    private Pump GetPump(int id) => _pumps.FirstOrDefault(p => p.Id.Equals(id))
         ?? throw new Exception("Pump doesn't exist");


    /// <summary>
    /// Devuelve el estado de todos los surtidores
    /// </summary>
    /// <returns>El estado de todos los surtidores</returns>
    public IEnumerable<PUMP_STATUS> GetPumpsStates() => _pumps.Select(p => p.Status);
}
