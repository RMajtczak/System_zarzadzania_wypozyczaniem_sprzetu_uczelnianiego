using AutoMapper;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<Equipment, EquipmentDto>();
        CreateMap<AddEquipmentDto, Equipment>();
        
        CreateMap<Borrowing, BorrowingDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name))
            .ForMember(dest => dest.BorrowerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<AddBorrowDto, Borrowing>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate == default ? DateTime.Now : src.StartDate));
    }
    
}