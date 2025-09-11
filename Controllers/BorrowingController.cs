using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Models;
using Wypożyczlania_sprzętu.Services;

namespace Wypożyczlania_sprzętu.Controllers;
[Route("api/borrowings")]
[ApiController]
[Authorize(Roles = "Admin,Manager")]
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
        return Ok(borrowing);
    }

    [HttpPost]
    public ActionResult AddBorrow([FromBody] AddBorrowDto dto)
    {
        var id = _borrowingService.AddBorrow(dto);
        return Created($"api/borrowing/{id}", null);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteBorrowing([FromRoute] int id)
    {
         _borrowingService.DeleteBorrowing(id);
        return NoContent();
    }

    [HttpPatch("{id}")]
    public ActionResult Return([FromRoute] int id)
    {
        _borrowingService.Return(id);
        return Ok("Zwrócono sprzęt");
    }
    [HttpGet("active")]
    public ActionResult<IEnumerable<BorrowingDto>> GetActiveBorrowings()
    {
        var activeBorrowings = _borrowingService.GetActiveBorrowings();
        return Ok(activeBorrowings);
    }
        
}

