
namespace GasStation.Interface;

public interface IPump
{
    PUMP_STATUS Status { get; set; }
    double? PredefinedMaxAmount { get; set; }
}
