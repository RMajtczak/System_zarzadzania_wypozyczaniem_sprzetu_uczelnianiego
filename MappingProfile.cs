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

        CreateMap<AddBorrowDto, Borrowing>();

        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name))
            .ForMember(dest => dest.BookerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

        CreateMap<CreateReservationDto, Reservation>();
        
        CreateMap<FaultReport, FaultReportDto>()
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));
        CreateMap<AddFaultReportDto, FaultReport>();

        CreateMap<RegisterUserDto, User>();

    }
    
}