using AutoMapper;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu;

public class EquipmentMappingProfile: Profile
{
    public EquipmentMappingProfile()
    {
        CreateMap<Equipment, EquipmentDto>();
        
    }
}