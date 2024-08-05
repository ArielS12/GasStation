
using GasStation.Model;

namespace GasStation.Interface;
public interface ISupply
{
    int PumpId { get; set; }
    double? PredefinedAmount { get; set; }
    double Amount { get; set; }
    DateTime CreatedDate { get; set; }
}
