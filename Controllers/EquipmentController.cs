using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route("api/equipment")]
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
}