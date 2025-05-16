namespace Wypożyczlania_sprzętu.Models;

public class AddBorrowDto
{
    public string EquipmentName { get; set; }
    public string BorrowerName { get; set; }
    public string Condition { get; set; }
    public DateTime StartDate { get; set; }
}