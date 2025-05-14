using Wypożyczlania_sprzętu.Entities;

namespace Wypożyczlania_sprzętu;

public class EquipmentSeeder
{
    private readonly RentalDbContext _dbContext;

    public EquipmentSeeder(RentalDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (_dbContext.Database.CanConnect())
        {
            if (!_dbContext.Users.Any())
            {
                var users = GetUsers();
                _dbContext.Users.AddRange(users);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Equipment.Any())
            {
                var equipment = GetEquipment();
                _dbContext.Equipment.AddRange(equipment);
                _dbContext.SaveChanges();
            }
        }
    }

    private IEnumerable<User> GetUsers()
    {
        var users =  new List<User>
        {
            new User
            {
                FirstName = "Rafał",
                LastName = "Majtczak",
                Email = "rafal.majtczak@gmail.com",
                Role = "Admin",
                Password = "admin123",
            },
            new User
            {
                FirstName = "Jan",
                LastName = "Kowalski",
                Email = "jan.kowalski@gmail.com",
                Role = "User",
                Password = "user123",
            }
        };
        return users;
    }
    private IEnumerable<Equipment> GetEquipment()
    {
        var equipment= new List<Equipment>
        {
            new Equipment
            {
                Name = "Laptop Lenovo",
                Type = "Laptop",
                SerialNumber = "LEN12345",
                Status = EquipmentStatus.Available,
                Location = "K 101",
                Specification = "Intel i7, 16GB RAM, 512GB SSD"
            },
            new Equipment
            {
                Name = "Camera Canon",
                Type = "Camera",
                SerialNumber = "CAN67890",
                Status = EquipmentStatus.Available,
                Location = "K 102",
                Specification = "24MP, 4K Video"
            }
        };
        return equipment;
    }
}