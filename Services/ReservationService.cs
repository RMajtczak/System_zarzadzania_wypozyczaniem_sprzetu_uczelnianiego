using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;

public interface IReservationService
{
    IEnumerable<ReservationDto> GetAllReservations();
    ReservationDto GetReservationById(int id);
    int CreateReservation(CreateReservationDto dto);
    bool CancelReservation(int id);
}

public class ReservationService : IReservationService
{
    private readonly IMapper _mapper;
    private readonly RentalDbContext _dbContext;

    public ReservationService(IMapper mapper, RentalDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public IEnumerable<ReservationDto> GetAllReservations()
    {
        var reservations = _dbContext.Reservations
            .Include(r => r.Equipment)
            .Include(r => r.User)
            .ToList();
        var reservationsDto = _mapper.Map<List<ReservationDto>>(reservations);
        return reservationsDto;
    }

    public ReservationDto GetReservationById(int id)
    {
        var reservation = _dbContext.Reservations
            .FirstOrDefault(r => r.Id == id);
        if (reservation == null) return null;
        var reservationDto = _mapper.Map<ReservationDto>(reservation);
        return reservationDto;
    }

    public int CreateReservation(CreateReservationDto dto)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Name == dto.EquipmentName);
        if (equipment == null || equipment.Status != EquipmentStatus.Available)
        {
            throw new InvalidOperationException("Sprzęt jest niedostępny lub nie istnieje.");
        }

        var user = _dbContext.Users.FirstOrDefault(u => (u.FirstName + " " + u.LastName) == dto.BookerName);
        if (user == null)
        {
            throw new InvalidOperationException("Użytkownik nie istnieje.");
        }

        var maxReservationDays = 14;
        var duration = (dto.EndDate - dto.StartDate).TotalDays;
        if (duration > maxReservationDays)
        {
            throw new InvalidOperationException($"Maksymalny czas rezerwacji to {maxReservationDays} dni.");
        }

        var reservation = _mapper.Map<Reservation>(dto);
        reservation.EquipmentId = equipment.Id;
        reservation.UserId = user.Id;
        reservation.StartDate = dto.StartDate;
        reservation.EndDate = dto.EndDate;
        reservation.IsCanceled = false;
        equipment.Status = EquipmentStatus.Reserved;
        _dbContext.Reservations.Add(reservation);
        _dbContext.SaveChanges();
        return reservation.Id;
    }
    public bool CancelReservation(int id)
    {
        var reservation = _dbContext.Reservations
            .Include(r => r.Equipment)
            .FirstOrDefault(r => r.Id == id);
        if (reservation == null)
        {
            throw new InvalidOperationException("Rezerwacja nie istnieje.");
        }
        if (reservation.IsCanceled)
        {
            throw new InvalidOperationException("Rezerwacja już została anulowana.");
        }
        if (reservation.EndDate < DateTime.Now)
        {
            throw new InvalidOperationException("Nie można anulować rezerwacji, która się już rozpoczęła.");
        }

        reservation.IsCanceled = true;
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Id == reservation.EquipmentId);
        if (equipment != null)
        {
            equipment.Status = EquipmentStatus.Available;
        }

        _dbContext.SaveChanges();
        return true;
    }
}