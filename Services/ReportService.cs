using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

public interface IReportService
{
    EquipmentReportDto GetEquipmentReport();
    byte[] ExportReportToCsv();
    byte[] ExportReportToPdf();
}

public class ReportService : IReportService
{
    private readonly RentalDbContext _dbContext;

    public ReportService(RentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Statystyki sprzętu
    public EquipmentReportDto GetEquipmentReport()
    {
        var equipments = _dbContext.Equipment.ToList();

        return new EquipmentReportDto
        {
            Total = equipments.Count,
            Dostępny = equipments.Count(e => e.Status == EquipmentStatus.Dostępny),
            Zarezerwowany = equipments.Count(e => e.Status == EquipmentStatus.Zarezerwowany),
            Wypożyczony = equipments.Count(e => e.Status == EquipmentStatus.Wypożyczony),
            Naprawa = equipments.Count(e => e.Status == EquipmentStatus.Naprawa),
            Uszkodzony = equipments.Count(e => e.Status == EquipmentStatus.Uszkodzony),
        };
    }

    // Eksport CSV
    public byte[] ExportReportToCsv()
    {
        var report = GetEquipmentReport();
        var sb = new StringBuilder();

        sb.AppendLine("Total,Dostępny,Zarezerwowany,Wypożyczony,Naprawa,Uszkodzony");
        sb.AppendLine($"{report.Total},{report.Dostępny},{report.Zarezerwowany},{report.Wypożyczony},{report.Naprawa},{report.Uszkodzony}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // Eksport PDF
    public byte[] ExportReportToPdf()
    {
        var report = GetEquipmentReport();

        using var ms = new MemoryStream();
        var writer = new PdfWriter(ms);

        // 1️⃣ Tworzymy plik PDF
        var pdf = new PdfDocument(writer);

        // 2️⃣ Tworzymy layout dokumentu
        var document = new Document(pdf);

        document.Add(new Paragraph("Raport sprzętu")
            .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
            .SetFontSize(16));
        document.Add(new Paragraph($"Data wygenerowania: {DateTime.Now}"));
        document.Add(new Paragraph("\n"));

        // Tworzymy tabelę
        var table = new Table(6, false);
        table.AddHeaderCell("Total");
        table.AddHeaderCell("Dostępny");
        table.AddHeaderCell("Zarezerwowany");
        table.AddHeaderCell("Wypożyczony");
        table.AddHeaderCell("Naprawa");
        table.AddHeaderCell("Uszkodzony");

        table.AddCell(report.Total.ToString());
        table.AddCell(report.Dostępny.ToString());
        table.AddCell(report.Zarezerwowany.ToString());
        table.AddCell(report.Wypożyczony.ToString());
        table.AddCell(report.Naprawa.ToString());
        table.AddCell(report.Uszkodzony.ToString());

        document.Add(table);
        document.Close();

        return ms.ToArray();
    }
}
