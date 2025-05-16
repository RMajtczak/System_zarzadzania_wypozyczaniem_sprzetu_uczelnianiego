using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;

public interface IEquipmentService
{
    IEnumerable<EquipmentDto> GetAllEquipment();
    EquipmentDto GetEquipmentById(int id);
    
    int CreateEquipment(AddEquipmentDto dto);
    bool UpdateEquipment(UpdateEquipmentDto dto, int id);
    bool UpdateEquipmentStatus(UpdateEquipmentStatusDto dto, int id);
    bool DeleteEquipment(int id);
    IEnumerable<EquipmentDto> FilterEquipment(EquipmentFilterDto filter);
}

public class EquipmentService : IEquipmentService
{
    private readonly IMapper _mapper;
    private readonly RentalDbContext _dbContext;
    

    public EquipmentService(IMapper mapper, RentalDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
        
    public IEnumerable<EquipmentDto> GetAllEquipment()
    {
        var equipment = _dbContext.Equipment.ToList();
        var equipmentDtos = _mapper.Map<List<EquipmentDto>>(equipment);
        return equipmentDtos;
    }
    public EquipmentDto GetEquipmentById(int id)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null) return null;
        var equipmentDto = _mapper.Map<EquipmentDto>(equipment);
        return equipmentDto;
    }
    public int CreateEquipment(AddEquipmentDto dto)
    {
        var equipment = _mapper.Map<Equipment>(dto);
        _dbContext.Equipment.Add(equipment);
        _dbContext.SaveChanges();
            
        return equipment.Id;
    }
    public bool UpdateEquipment(UpdateEquipmentDto dto, int id)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null) return false;
        equipment.Name = dto.Name;
        equipment.Type = dto.Type;
        equipment.Specification = dto.Specification;
        equipment.Location = dto.Location;
        _dbContext.SaveChanges();
        return true;
    }
    public bool UpdateEquipmentStatus(UpdateEquipmentStatusDto dto, int id)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null) return false;
        equipment.Status = dto.Status;
        _dbContext.SaveChanges();
        return true;
    }
    public bool DeleteEquipment(int id)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Id == id);
        if (equipment == null) return false;
        _dbContext.Equipment.Remove(equipment);
        _dbContext.SaveChanges();
        return true;
    }

    public IEnumerable<EquipmentDto> FilterEquipment(EquipmentFilterDto filter)
    {
        var query = _dbContext.Equipment.AsQueryable();
        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(e => e.Name.Contains(filter.Name));
        }
        if (!string.IsNullOrEmpty(filter.Type))
        {
            query = query.Where(e => e.Type.Contains(filter.Type));
        }
        if (!string.IsNullOrEmpty(filter.Location))
        {
            query = query.Where(e => e.Location.Contains(filter.Location));
        }
        if (filter.Status != null)
        {
            query = query.Where(e => e.Status == filter.Status);
        }
        if (!string.IsNullOrEmpty(filter.SerialNumber))
        {
            query = query.Where(e => e.SerialNumber.Contains(filter.SerialNumber));
        }
        var equipment = query.ToList();
        return _mapper.Map<List<EquipmentDto>>(equipment);
    } 
    
}