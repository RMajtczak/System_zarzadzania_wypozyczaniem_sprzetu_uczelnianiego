using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Wypożyczlania_sprzętu.Entities;
using Wypożyczlania_sprzętu.Models;

namespace Wypożyczlania_sprzętu.Services;
public interface IBorrowingService
{
    IEnumerable<BorrowingDto> GetAllBorrowings();
    BorrowingDto GetBorrowingById(int id);
    int AddBorrow(AddBorrowDto dto);
    bool DeleteBorrowing(int id);

}
public class BorrowingService : IBorrowingService
{
    private readonly IMapper _mapper;
    private readonly RentalDbContext _dbContext;
    private IBorrowingService _borrrowingServiceImplementation;


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
        var borrowingDtos = _mapper.Map<List<BorrowingDto>>(borrowings);
        return borrowingDtos;
    }
    public BorrowingDto GetBorrowingById(int id)
    {
        var borrowing = _dbContext.Borrowings.FirstOrDefault(b => b.Id == id);
        if (borrowing == null) return null;
        var borrowingDto = _mapper.Map<BorrowingDto>(borrowing);
        return borrowingDto;
    }

    public int AddBorrow(AddBorrowDto dto)
    {
        var equipment = _dbContext.Equipment.FirstOrDefault(e => e.Name == dto.EquipmentName);
        if (equipment == null || equipment.Status != EquipmentStatus.Available)
        {
            throw new InvalidOperationException("Sprzęt jest niedostępny lub nie istnieje.");
        }

        var user = _dbContext.Users.FirstOrDefault(u => (u.FirstName + " " + u.LastName) == dto.BorrowerName);
        if (user == null)
        {
            throw new InvalidOperationException("Użytkownik nie istnieje.");
        }

        var borrowing = _mapper.Map<Borrowing>(dto);
        borrowing.EquipmentId = equipment.Id;
        borrowing.UserId = user.Id;
        borrowing.IsReturned = false;
        

        equipment.Status = EquipmentStatus.Borrowed;

        _dbContext.Borrowings.Add(borrowing);
        _dbContext.SaveChanges();

        return borrowing.Id;
    }

    public bool DeleteBorrowing(int id)
    {
        var borrowing = _dbContext.Borrowings.FirstOrDefault(r => r.Id == id);
        if (borrowing == null) return false;
        _dbContext.Borrowings.Remove(borrowing);
        _dbContext.SaveChanges();
        return true;

    }
}