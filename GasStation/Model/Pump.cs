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
}
