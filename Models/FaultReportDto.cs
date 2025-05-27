namespace Wypożyczlania_sprzętu.Models;

public class FaultReportDto
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public string UserName { get; set; }
    public string EquipmentName { get; set; }
    public string Description { get; set; }
    public DateTime ReportDate { get; set; }
    public bool IsResolved { get; set; }
    
}