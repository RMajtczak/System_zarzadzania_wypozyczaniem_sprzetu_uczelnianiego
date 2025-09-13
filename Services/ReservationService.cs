using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Exceptions;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;

public interface IReservationService
{
    IEnumerable<ReservationDto> GetReservationsByUserId(int userId);
    ReservationDto GetReservationById(int id);
    int CreateReservation(CreateReservationDto dto, string userName);
    void CancelReservation(int id);
    void CloseExpiredReservations();
    IEnumerable<ReservationDto> GetActiveReservations(string userId);

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
    public IEnumerable<ReservationDto> GetReservationsByUserId(int userId)
    {
        var now = DateTime.UtcNow;

        var reservations = _dbContext.Reservations
            .Where(r => r.UserId == userId && r.StartDate > now)
            .Include(r => r.Equipment)
            .ToList();

        var result = _mapper.Map<List<ReservationDto>>(reservations);
        return result;
    }
    public IEnumerable<ReservationDto> GetActiveReservations(string userId)
    {
        var reservations = _dbContext.Reservations
            .Include(r => r.Equipment)
            .Include(r => r.User)
            .Where(r => r.User.Id == int.Parse(userId) && !r.IsCanceled)
            .ToList();

        return _mapper.Map<List<ReservationDto>>(reservations);
    }

    public ReservationDto GetReservationById(int id)
    {
        var reservation = _dbContext.Reservations
            .FirstOrDefault(r => r.Id == id);
        if (reservation == null) 
            throw new NotFoundException("Nie znaleziono rezerwacji.");
        var reservationDto = _mapper.Map<ReservationDto>(reservation);
        return reservationDto;
    }

    public int CreateReservation(CreateReservationDto dto, string userName)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Name == dto.EquipmentName);
        if (equipment == null || equipment.Status != EquipmentStatus.Dostępny)
        {
            throw new NotFoundException("Sprzęt jest niedostępny lub nie istnieje.");
        }

        var user = _dbContext.Users.FirstOrDefault(u => (u.FirstName + " " + u.LastName) == dto.BookerName);
        if (user == null)
        {
            throw new NotFoundException("Użytkownik nie istnieje.");
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
        equipment.Status = EquipmentStatus.Zarezerwowany;
        _dbContext.Reservations.Add(reservation);
        _dbContext.SaveChanges();
        return reservation.Id;
    }
    public void CancelReservation(int id)
    {
        var reservation = _dbContext.Reservations
            .Include(r => r.Equipment)
            .FirstOrDefault(r => r.Id == id);
        if (reservation == null)
        {
            throw new NotFoundException("Rezerwacja nie istnieje.");
        }
        if (reservation.IsCanceled)
        {
            throw new BadRequestException("Rezerwacja została już anulowana.");
        }
        if (reservation.EndDate < DateTime.Now)
        {
            throw new BadRequestException("Nie można anulować rezerwacji, która się już rozpoczęła.");
        }

        reservation.IsCanceled = true;
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Id == reservation.EquipmentId);
        if (equipment != null)
        {
            equipment.Status = EquipmentStatus.Dostępny;
        }

        _dbContext.SaveChanges();
        
    }

    public void CloseExpiredReservations()
    {
        var now = DateTime.UtcNow;
        var expiredReservations = _dbContext.Reservations
            .Include(r=> r.Equipment)
            .Where(r => r.EndDate < now && !r.IsCanceled)
            .ToList();
        foreach (var reservation in expiredReservations)
        {
            reservation.IsCanceled = true;

            if (reservation.Equipment != null)
            {
                reservation.Equipment.Status = EquipmentStatus.Dostępny;
            }
        }

        _dbContext.SaveChanges();
    }
    
}