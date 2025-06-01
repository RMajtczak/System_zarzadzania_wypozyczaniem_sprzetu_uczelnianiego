using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Exceptions;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;
public interface IBorrowingService
{
    IEnumerable<BorrowingDto> GetAllBorrowings();
    BorrowingDto GetBorrowingById(int id);
    int AddBorrow(AddBorrowDto dto);
    void DeleteBorrowing(int id);
    void Return(int id);

}
public class BorrowingService : IBorrowingService
{
    private readonly IMapper _mapper;
    private readonly RentalDbContext _dbContext;


    public BorrowingService(IMapper mapper, RentalDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    public IEnumerable<BorrowingDto> GetAllBorrowings()
    {
        var borrowings = _dbContext.Borrowings
            .Include(b => b.Equipment)
            .Include(b => b.User)
            .ToList();
        var borrowingDto = _mapper.Map<List<BorrowingDto>>(borrowings);
        return borrowingDto;
    }
    public BorrowingDto GetBorrowingById(int id)
    {
        var borrowing = _dbContext.Borrowings.FirstOrDefault(b => b.Id == id);
        if (borrowing == null) 
            throw new NotFoundException("Nie znaleziono wypożycznia");
        var borrowingDto = _mapper.Map<BorrowingDto>(borrowing);
        return borrowingDto;
    }

    public int AddBorrow(AddBorrowDto dto)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Name == dto.EquipmentName);
        if (equipment == null || equipment.Status != EquipmentStatus.Available)
        {
            throw new NotFoundException("Sprzęt jest niedostępny lub nie istnieje.");
        }

        var user = _dbContext.Users.FirstOrDefault(u => (u.FirstName + " " + u.LastName) == dto.BorrowerName);
        if (user == null)
        {
            throw new NotFoundException("Użytkownik nie istnieje.");
        }

        var maxRentalDays = 14;
        var duration = (dto.EndDate - dto.StartDate).TotalDays;
        if (duration > maxRentalDays)
            throw new InvalidOperationException($"Maksymalny czas wypożyczenia to {maxRentalDays} dni.");
        var borrowing = _mapper.Map<Borrowing>(dto);
        borrowing.EquipmentId = equipment.Id;
        borrowing.UserId = user.Id;
        borrowing.StartDate = dto.StartDate;
        borrowing.EndDate = dto.EndDate;
        borrowing.IsReturned = false;
        
        equipment.Status = EquipmentStatus.Borrowed;

        _dbContext.Borrowings.Add(borrowing);
        _dbContext.SaveChanges();

        return borrowing.Id;
    }

    public void DeleteBorrowing(int id)
    {
        var borrowing = _dbContext.Borrowings.FirstOrDefault(r => r.Id == id);
        if (borrowing == null) 
            throw new NotFoundException("Nie znaleziono wypożycznia");
        _dbContext.Borrowings.Remove(borrowing);
        _dbContext.SaveChanges();
        
    }

    public void Return(int id)
    {
        var borrowing = _dbContext.Borrowings
            .Include(b => b.Equipment)
            .FirstOrDefault(r => r.Id == id);
        if (borrowing == null)
            throw new NotFoundException("Nie znaleziono wypożycznia");
        if (borrowing.IsReturned)
            throw new NotFoundException("Przedmiot jest już zwrócony");
        borrowing.IsReturned = true;
        borrowing.EndDate = DateTime.Now;
        borrowing.Equipment.Status = EquipmentStatus.Available;
        _dbContext.SaveChanges();
    }
}