namespace Wypożyczlania_sprzętu.Models;

public class BorrowingDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; }
    public string BorrowerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Condition { get; set; }
    public bool IsReturned { get; set; }
}