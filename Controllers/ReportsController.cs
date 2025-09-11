using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;

[ApiController]
[Route("api/reports")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("equipment-stats")]
    public ActionResult<EquipmentReportDto> GetEquipmentStats()
    {
        return Ok(_reportService.GetEquipmentReport());
    }

    [HttpGet("equipment-report/csv")]
    public IActionResult GetReportCsv()
    {
        var file = _reportService.ExportReportToCsv();
        return File(file, "text/csv", "raport_sprzetu.csv");
    }

    [HttpGet("equipment-report/pdf")]
    public IActionResult GetReportPdf()
    {
        var file = _reportService.ExportReportToPdf();
        return File(file, "application/pdf", "raport_sprzetu.pdf");
    }
}