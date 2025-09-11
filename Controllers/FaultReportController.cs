using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;

[Route("api/faultreports")]
[ApiController]
[Authorize(Roles = "Admin,Manager,User")]
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
        return Ok(faultReport);
    }

    [HttpPost]
    public ActionResult CreateFaultReport([FromBody] AddFaultReportDto dto)
    {
        var id = _faultReportService.CreateFaultReport(dto);
        return Created($"/api/faultreports/{id}", null);
    }

    [HttpPatch("{id}/resolve")]
    public ActionResult ResolveFaultReport([FromRoute] int id)
    {
        _faultReportService.ResolveFaultReport(id);
        return Ok();
    }
}