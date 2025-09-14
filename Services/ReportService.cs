using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Text;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

public interface IReportService
{
    byte[] GenerateBorrowingsCsv(DateTime? start, DateTime? end);
    byte[] GenerateBorrowingsPdf(DateTime? start, DateTime? end);
    byte[] GenerateTopEquipmentCsv(int topN);
    byte[] GenerateTopEquipmentPdf(int topN);
    List<BorrowingReportDto> GetBorrowings(DateTime? start, DateTime? end);
    List<TopEquipmentDto> GetTopEquipment(int topN);
}

public class ReportService : IReportService
{
    private readonly RentalDbContext _context;

    public ReportService(RentalDbContext context)
    {
        _context = context;
    }

    // =====================
    // Borrowings → CSV
    // =====================
    public byte[] GenerateBorrowingsCsv(DateTime? start, DateTime? end)
    {
        var data = GetBorrowings(start, end);

        var csv = new StringBuilder();
        csv.AppendLine("User;Equipment;StartDate;EndDate;Status");

        foreach (var b in data)
        {
            var userName = !string.IsNullOrWhiteSpace(b.User) ? b.User : "Unknown User";
            
            var equipmentName = !string.IsNullOrWhiteSpace(b.Equipment) ? b.Equipment : "Unknown";
            
            var startDate = b.StartDate.ToString("dd.MM.yyyy");
            var endDate = b.EndDate.HasValue ? b.EndDate.Value.ToString("dd.MM.yyyy") : "-";
            
            var status = b.IsReturned ? "Returned" : "Active";

            csv.AppendLine($"{userName};{equipmentName};{startDate};{endDate};{status}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }


    // =====================
    // Borrowings → PDF
    // =====================
    public byte[] GenerateBorrowingsPdf(DateTime? start, DateTime? end)
    {
        var data = GetBorrowings(start, end);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text("Raport wypożyczeń")
                    .FontSize(20).Bold().AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2); // User
                        columns.RelativeColumn(2); // Equipment
                        columns.RelativeColumn();  // StartDate
                        columns.RelativeColumn();  // EndDate
                        columns.RelativeColumn();  // Status
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("User").Bold();
                        header.Cell().Text("Equipment").Bold();
                        header.Cell().Text("Start Date").Bold();
                        header.Cell().Text("End Date").Bold();
                        header.Cell().Text("Status").Bold();
                    });

                    foreach (var b in data)
                    {
                        var userName = !string.IsNullOrWhiteSpace(b.User) ? b.User : "Unknown User";
                        var equipmentName = !string.IsNullOrWhiteSpace(b.Equipment) ? b.Equipment : "Unknown";
                        var startDate = b.StartDate.ToString("dd.MM.yyyy");
                        var endDate = b.EndDate.HasValue ? b.EndDate.Value.ToString("dd.MM.yyyy") : "-";
                        var status = b.IsReturned ? "Returned" : "Active";

                        table.Cell().Text(userName);
                        table.Cell().Text(equipmentName);
                        table.Cell().Text(startDate);
                        table.Cell().Text(endDate);
                        table.Cell().Text(status);
                    }
                });
            });
        }).GeneratePdf();
    }


    // =====================
    // Top Equipment → CSV
    // =====================
    public byte[] GenerateTopEquipmentCsv(int topN)
    {
        var data = GetTopEquipment(topN);

        var csv = new StringBuilder();
        csv.AppendLine("Equipment;Count");

        foreach (var item in data)
        {
            var equipmentName = !string.IsNullOrWhiteSpace(item.Equipment) ? item.Equipment : "Unknown";
            var count = item.Count;

            csv.AppendLine($"{equipmentName};{count}");
        }

        return Encoding.UTF8.GetBytes(csv.ToString());
    }


    // =====================
    // Top Equipment → PDF
    // =====================
    public byte[] GenerateTopEquipmentPdf(int topN)
    {
        var data = GetTopEquipment(topN);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Header().Text($"TOP {topN} najczęściej wypożyczanych sprzętów")
                    .FontSize(20).Bold().AlignCenter();

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Equipment").Bold();
                        header.Cell().Text("Count").Bold();
                    });

                    foreach (var item in data)
                    {
                        table.Cell().Text(item.Equipment);
                        table.Cell().Text(item.Count.ToString());
                    }
                });
            });
        }).GeneratePdf();
    }

    // =====================
    // Helpers
    // =====================
    public List<BorrowingReportDto> GetBorrowings(DateTime? start, DateTime? end)
    {
        var query = _context.Borrowings
            .Include(b => b.User)
            .Include(b => b.Equipment)
            .AsQueryable();

        if (start.HasValue)
            query = query.Where(b => b.StartDate >= start);

        if (end.HasValue)
            query = query.Where(b => b.EndDate <= end);

        return query.Select(b => new BorrowingReportDto
        {
            User = b.User.FirstName + " " + b.User.LastName,
            Equipment = b.Equipment.Name,
            StartDate = b.StartDate,
            EndDate = b.EndDate,
            IsReturned = b.IsReturned
        }).ToList();
    }

    public List<TopEquipmentDto> GetTopEquipment(int topN)
    {
        return _context.Borrowings
            .Include(b => b.Equipment)
            .GroupBy(b => b.Equipment.Name)
            .Select(g => new TopEquipmentDto
            {
                Equipment = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .Take(topN)
            .ToList();
    }
}
