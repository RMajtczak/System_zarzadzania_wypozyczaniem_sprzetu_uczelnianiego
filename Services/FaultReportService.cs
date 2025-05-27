using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;

public interface IFaultReportService
{
    IEnumerable<FaultReportDto> GetAllFaultReports();
    FaultReportDto GetFaultReportById(int id);
    int CreateFaultReport(AddFaultReportDto dto);
    bool ResolveFaultReport(int id);
}

public class FaultReportService : IFaultReportService
    {
        private readonly IMapper _mapper;
        private readonly RentalDbContext _dbContext;

        public FaultReportService(IMapper mapper, RentalDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public IEnumerable<FaultReportDto> GetAllFaultReports()
        {
            var faultReports = _dbContext.FaultReports
                .Include(fr => fr.User)
                .Include(fr => fr.Equipment)
                .ToList();
            var faultReportsDto = _mapper.Map<List<FaultReportDto>>(faultReports);
            return faultReportsDto;
        }

        public FaultReportDto GetFaultReportById(int id)
        {
            var faultReport = _dbContext.FaultReports
                .Include(fr => fr.User)
                .Include(fr => fr.Equipment)
                .FirstOrDefault(fr => fr.Id == id);
            if (faultReport == null) return null;
            var faultReportDto = _mapper.Map<FaultReportDto>(faultReport);
            return faultReportDto;
        }

        public int CreateFaultReport(AddFaultReportDto dto)
        {
            var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Name == dto.EquipmentName);
            if (equipment == null || equipment.Status != EquipmentStatus.Available)
            {
                throw new InvalidOperationException("Sprzęt jest niedostępny lub nie istnieje.");
            }

            var user = _dbContext.Users.FirstOrDefault(u => (u.FirstName + " " + u.LastName) == dto.UserName);
            if (user == null)
            {
                throw new InvalidOperationException("Użytkownik nie istnieje.");
            }
            var faultReport = _mapper.Map<FaultReport>(dto);
            faultReport.EquipmentId = equipment.Id;
            faultReport.UserId = user.Id;
            faultReport.ReportDate = DateTime.Now;
            faultReport.Description = dto.Description;
            faultReport.IsResolved = false;
            
            _dbContext.FaultReports.Add(faultReport);
            _dbContext.SaveChanges();
            return faultReport.Id;
            
        }

        public bool ResolveFaultReport(int id)
        {
            var faultReport = _dbContext.FaultReports.FirstOrDefault(fr => fr.Id == id);
            if (faultReport == null) return false;

            faultReport.IsResolved = true;
            _dbContext.SaveChanges();

            return true;
        }
    }
