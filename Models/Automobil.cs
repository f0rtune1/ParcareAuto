namespace ParcareAuto.Models;

public class Automobil
{
    public string NumarInmatriculare { get; set; } = "";
    public string Marca { get; set; } = "";
    public string Tip { get; set; } = "";
    public DateTime OraIntrare { get; set; }
    public int OreStationare { get; set; }
    public decimal TarifOra { get; set; }

    public decimal CostParcare => OreStationare * TarifOra;
}
