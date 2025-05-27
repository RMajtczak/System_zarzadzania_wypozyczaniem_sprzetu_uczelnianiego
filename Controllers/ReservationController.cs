using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route ("api/reservations")]
public class ReservationController: ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }
    [HttpGet]
    public ActionResult<IEnumerable<ReservationDto>> GetAll()
    {
        var reservations = _reservationService.GetAllReservations();
        return Ok(reservations);
    }

    [HttpGet("{id}")]
    public ActionResult<ReservationDto> GetById([FromRoute] int id)
    {
        var reservation = _reservationService.GetReservationById(id);
        if (reservation == null)
        {
            return NotFound("Nie znaleziono rezerwacji");
        }

        return Ok(reservation);
    }
    [HttpPost ]
    public ActionResult Create([FromBody] CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var id = _reservationService.CreateReservation(dto);
        return Created($"/api/reservations/{id}", null);
    }
    [HttpPost("{id}/cancel")]
    public ActionResult CancelReservation([FromRoute] int id)
    {
        try
        {
            _reservationService.CancelReservation(id);
            return Ok("Rezerwacja została anulowana.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}