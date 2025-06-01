using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route("api/equipment")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }
    [HttpGet]
    public ActionResult<IEnumerable<EquipmentDto>> GetAll()
    {
        var equipmentDtos = _equipmentService.GetAllEquipment();
        return Ok(equipmentDtos);
    }
    [HttpGet("{id}")]
    public ActionResult<EquipmentDto> GetById([FromRoute] int id)
    {
        var equipment = _equipmentService.GetEquipmentById(id);
        return Ok(equipment);
    }
    [HttpPost]
    public ActionResult CreateEquipment([FromBody] AddEquipmentDto dto)
    {

        var id = _equipmentService.CreateEquipment(dto);
        return Created($"/api/equipment/{id}", null);
    }
    [HttpPut("{id}")]
    public ActionResult UpdateEquipment([FromBody] UpdateEquipmentDto dto, [FromRoute] int id)
    {
        _equipmentService.UpdateEquipment(dto, id);
        return Ok();
    }
    [HttpPatch("{id}")]
    public ActionResult UpdateEquipmentStatus([FromBody] UpdateEquipmentStatusDto dto, [FromRoute] int id)
    {
        _equipmentService.UpdateEquipmentStatus(dto, id);
        return Ok();
    }
    [HttpDelete("{id}")]
    public ActionResult DeleteEquipment([FromRoute] int id)
    {
        _equipmentService.DeleteEquipment(id);
        return NoContent();
    }
    [HttpGet("search")]
    public ActionResult<IEnumerable<EquipmentDto>> FilterEquipment([FromQuery] EquipmentFilterDto filter)
    {
        var equipmentDtos = _equipmentService.FilterEquipment(filter);
        return Ok(equipmentDtos);
    }
}