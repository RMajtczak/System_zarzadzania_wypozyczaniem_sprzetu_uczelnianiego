namespace Wypożyczlania_sprzętu.Models;

public class BorrowingReportDto
{
    public string User { get; set; }
    public string Equipment { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsReturned { get; set; }
}