using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;

[Route("api/faultreports")]
public class FaultReportController : ControllerBase
{
    private readonly IFaultReportService _faultReportService;

    public FaultReportController(IFaultReportService faultReportService)
    {
        _faultReportService = faultReportService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<FaultReportDto>> GetAll()
    {
        var faultReports = _faultReportService.GetAllFaultReports();
        return Ok(faultReports);
    }

    [HttpGet("{id}")]
    public ActionResult<FaultReportDto> GetById([FromRoute] int id)
    {
        var faultReport = _faultReportService.GetFaultReportById(id);
        if (faultReport == null)
        {
            return NotFound("Fault report not found");
        }
        return Ok(faultReport);
    }

    [HttpPost]
    public ActionResult CreateFaultReport([FromBody] AddFaultReportDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var id = _faultReportService.CreateFaultReport(dto);
        return Created($"/api/faultreports/{id}", null);
    }

    [HttpPatch("{id}/resolve")]
    public ActionResult ResolveFaultReport([FromRoute] int id)
    {
        var isResolved = _faultReportService.ResolveFaultReport(id);
        if (!isResolved)
        {
            return NotFound("Fault report not found");
        }
        return Ok();
    }
}