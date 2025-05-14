namespace Wypożyczlania_sprzętu.Entities;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string SerialNumber { get; set; }
    public string Specification { get; set; }
    public string Location { get; set; }
    
    public EquipmentStatus Status { get; set; }
    
    public virtual List<Reservation> Reservations { get; set; }
    public virtual List<Borrowing> Borrowings { get; set; }
    public virtual List<FaultReport> FaultReports { get; set; }
}