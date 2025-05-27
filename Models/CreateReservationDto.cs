
namespace Wypożyczlania_sprzętu.Models;

public class CreateReservationDto
{
    public string EquipmentName { get; set; }
    public string BookerName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
}