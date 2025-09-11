namespace Wypożyczlania_sprzętu.Models;

public class EquipmentReportDto
{
    public int Total { get; set; }
    public int Dostępny { get; set; }
    public int Zarezerwowany { get; set; }
    public int Wypożyczony { get; set; }
    public int Naprawa { get; set; }
    public int Uszkodzony { get; set; }
}