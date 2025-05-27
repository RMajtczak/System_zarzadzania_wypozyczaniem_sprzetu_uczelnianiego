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
            if (!_dbContext.Equipment.Any())
            {
                var equipment = GetEquipment();
                _dbContext.Equipment.AddRange(equipment);
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Roles.Any())
            {
                var roles = GetRoles();
                _dbContext.Roles.AddRange(roles);
                _dbContext.SaveChanges();
            }
        }
    }
    private IEnumerable<Role> GetRoles()
    {
        var roles = new List<Role>
        {
            new Role() { Name = "User" },
            new Role() { Name = "Menager" },
            new Role() { Name = "Admin" }
        };
        return roles;
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