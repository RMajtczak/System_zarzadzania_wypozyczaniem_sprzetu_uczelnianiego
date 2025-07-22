using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route ("api/reservations")]
[ApiController]
[Authorize]
public class ReservationController: ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<ReservationDto>> GetOwn()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reservations = _reservationService.GetReservationsByUserId(int.Parse(userId));
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public ActionResult<ReservationDto> GetById([FromRoute] int id)
    {
        var reservation = _reservationService.GetReservationById(id);
        return Ok(reservation);
    }
    
    [HttpPost]
    public ActionResult Create([FromBody] CreateReservationDto dto)
    {
        var userName = User.Identity?.Name;

        if (string.IsNullOrEmpty(userName))
        {
            return Unauthorized("Brak autoryzacji");
        }
        var id = _reservationService.CreateReservation(dto, userName);
        return Created($"/api/reservations/{id}", null);
    }
    
    [HttpPost("{id}/cancel")]
    public ActionResult CancelReservation([FromRoute] int id)
    {
        _reservationService.CancelReservation(id);
        return Ok("Rezerwacja została anulowana.");
    }
}