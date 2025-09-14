using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }


    [HttpGet("borrowings/csv")]
    public IActionResult BorrowingsCsv(DateTime? startDate, DateTime? endDate)
    {
        var file = _reportService.GenerateBorrowingsCsv(startDate, endDate);
        return File(file, "text/csv", "BorrowingsReport.csv");
    }


    [HttpGet("borrowings/pdf")]
    public IActionResult BorrowingsPdf(DateTime? startDate, DateTime? endDate)
    {
        try
    {
        var pdfData = _reportService.GenerateBorrowingsPdf(startDate, endDate);
        return File(pdfData, "application/pdf", "BorrowingsReport.pdf");
    }
    catch (Exception ex)
    {
        // Tylko do testów – pokaże prawdziwy błąd w JSON
        return BadRequest(new { error = ex.Message, stack = ex.StackTrace });
    }
    }


    [HttpGet("top-equipment/csv")]
    public IActionResult TopEquipmentCsv(int topN = 5)
    {
        var file = _reportService.GenerateTopEquipmentCsv(topN);
        return File(file, "text/csv", $"Top{topN}Equipment.csv");
    }
    
    [HttpGet("top-equipment/pdf")]
    public IActionResult TopEquipmentPdf(int topN = 5)
    {
        var file = _reportService.GenerateTopEquipmentPdf(topN);
        return File(file, "application/pdf", $"Top{topN}Equipment.pdf");
    }
    
    [HttpGet("borrowings")]
    public IActionResult BorrowingsData(DateTime? startDate, DateTime? endDate)
    {
        var data = _reportService.GetBorrowings(startDate, endDate); 
        return Ok(data);
    }
    [HttpGet("top-equipment")]
    public IActionResult TopEquipmentData(int topN = 5)
    {
        var data = _reportService.GetTopEquipment(topN); // metoda publiczna w serwisie
        return Ok(data);
    }
}