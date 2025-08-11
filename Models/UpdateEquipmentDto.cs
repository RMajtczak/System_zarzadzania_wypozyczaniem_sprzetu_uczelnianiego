using Wypożyczlania_sprzętu.Entities;

namespace Wypożyczlania_sprzętu.Models;

public class UpdateEquipmentDto
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string SerialNumber { get; set; }
    public string Specification { get; set; }
    public string Location { get; set; }
    public EquipmentStatus Status { get; set; }
    
}