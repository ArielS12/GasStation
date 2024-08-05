using GasStation.Interface;

namespace GasStation.Model;
public class Supply : ISupply
{
    public int Id { get; set; }
    public int PumpId { get; set; }
    public double? PredefinedAmount { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedDate { get; set; }

}


