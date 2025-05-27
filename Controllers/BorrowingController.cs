using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route("api/borrowing")]
public class BorrowingController : ControllerBase
{
    private readonly IBorrowingService _borrowingService;

    public BorrowingController(IBorrowingService borrowingService)
    {
        _borrowingService = borrowingService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<BorrowingDto>> GetAll()
    {
        var borrowings = _borrowingService.GetAllBorrowings();
        return Ok(borrowings);
    }

    [HttpGet("{id}")]
    public ActionResult<BorrowingDto> GetById([FromRoute] int id)
    {
        var borrowing = _borrowingService.GetBorrowingById(id);
        if (borrowing == null)
        {
            return NotFound("Borrowing not found");
        }

        return Ok(borrowing);
    }

    [HttpPost]
    public ActionResult AddBorrow([FromBody] AddBorrowDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var id = _borrowingService.AddBorrow(dto);
        return Created($"api/borrowing/{id}", null);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteBorrowing([FromRoute] int id)
    {
        var IsDeleted = _borrowingService.DeleteBorrowing(id);
        if (!IsDeleted)
        {
            return NotFound("Nie znaleziono wypożyczenia.");
        }

        return NoContent();
    }

    [HttpPatch("{id}")]
    public ActionResult Return([FromRoute] int id)
    {
        var IsReturned = _borrowingService.Return(id);
        if (!IsReturned)
        {
            return BadRequest();
        }

        return Ok("Zwrócono sprzęt");
    }
    
        
}

