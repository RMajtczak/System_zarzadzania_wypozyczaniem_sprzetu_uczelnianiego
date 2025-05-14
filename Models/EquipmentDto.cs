using Wypożyczlania_sprzętu.Entities;

namespace Wypożyczlania_sprzętu.Models;

public class EquipmentDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Specification { get; set; }
    public EquipmentStatus Status { get; set; }
    
}