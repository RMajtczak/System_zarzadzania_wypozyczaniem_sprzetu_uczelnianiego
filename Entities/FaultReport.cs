namespace Wypożyczlania_sprzętu.Entities;

public class FaultReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    public string Description { get; set; }
    public DateTime ReportDate { get; set; }
    public bool IsResolved { get; set; }
}