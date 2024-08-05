/// <summary>
/// possible status of the pump
/// </summary>
public enum PUMP_STATUS
{
    BLOCKED = 1,
    FREE,
    CHARGING,
}

/// <summary>
/// Actions that a pump can do
/// </summary>
public enum PUMP_ACTION { 
    BLOCK = 1,
    RELEASE,
    START_CHARGING,
    STOP_CHARGING
}
