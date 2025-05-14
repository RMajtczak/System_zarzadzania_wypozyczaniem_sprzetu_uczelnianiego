namespace Wypożyczlania_sprzętu.Entities;

public class Borrowing
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string ConditionOnReturn { get; set; }
}