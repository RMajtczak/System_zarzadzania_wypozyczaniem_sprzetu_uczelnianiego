using AutoMapper;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;

public interface IEquipmentService
{
    IEnumerable<EquipmentDto> GetAllEquipment();
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
}