namespace Wypożyczlania_sprzętu.Models;

public class ReservationDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; }
    public string BookerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}