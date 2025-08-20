using Microsoft.AspNetCore.Mvc;
using Wypożyczlania_sprzętu.Services;
using System.Collections.Generic;
using System.Linq;

namespace Wypożyczlania_sprzętu.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IBorrowingService _borrowingService;

        public ReportsController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }
        
        [HttpGet("borrowings")]
        public ActionResult<IEnumerable<object>> GetBorrowingReport()
        {
            var borrowings = _borrowingService.GetAllBorrowings();

            var report = borrowings.Select(b => new
            {
                EquipmentName = b.EquipmentName,
                BorrowerName = b.BorrowerName,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                Condition = b.Condition,
                IsReturned = b.IsReturned
            }).ToList();

            return Ok(report);
        }

        // GET: api/reports/stats/equipment
        [HttpGet("stats/equipment")]
        public ActionResult<IEnumerable<object>> GetEquipmentStats()
        {
            var borrowings = _borrowingService.GetAllBorrowings();

            var stats = borrowings
                .GroupBy(b => b.EquipmentName)
                .Select(g => new
                {
                    EquipmentName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return Ok(stats);
        }
    }
}