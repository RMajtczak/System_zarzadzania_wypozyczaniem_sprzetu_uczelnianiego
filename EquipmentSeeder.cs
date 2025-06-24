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
            new Role() { Name = "Manager" },
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
                Status = EquipmentStatus.Dostępny,
                Location = "K 101",
                Specification = "Intel i7, 16GB RAM, 512GB SSD"
            },
            new Equipment
            {
                Name = "Camera Canon",
                Type = "Camera",
                SerialNumber = "CAN67890",
                Status = EquipmentStatus.Dostępny,
                Location = "K 102",
                Specification = "24MP, 4K Video"
            },
            new Equipment
            {
                Name = "Projector Epson",
                Type = "Projektor",
                SerialNumber = "EPS54321",
                Status = EquipmentStatus.Dostępny,
                Location = "K 103",
                Specification = "Full HD, 3000 Lumens"
            },
            new Equipment
            {
                Name = "Aparat Canon",
                Type = "Aparat",
                SerialNumber = "CAN67891",
                Status = EquipmentStatus.Dostępny,
                Location = "K 102",
                Specification = "24MP, Wideo 4K"
            },
            new Equipment
            {
                Name = "Tablet Samsung Galaxy Tab",
                Type = "Tablet",
                SerialNumber = "SAM12345",
                Status = EquipmentStatus.Dostępny,
                Location = "K 104",
                Specification = "10.4\", 64GB, Wi-Fi"
            },
            new Equipment
            {
                Name = "Drukarka HP LaserJet",
                Type = "Drukarka",
                SerialNumber = "HP54321",
                Status = EquipmentStatus.Dostępny,
                Location = "K 106",
                Specification = "Laserowa, Duplex, Wi-Fi"
            },
            new Equipment
            {
                Name = "Monitor Dell UltraSharp",
                Type = "Monitor",
                SerialNumber = "DEL98765",
                Status = EquipmentStatus.Dostępny,
                Location = "K 107",
                Specification = "27\", QHD, IPS"
            },
            new Equipment
            {
                Name = "Klawiatura Logitech MX Keys",
                Type = "Klawiatura",
                SerialNumber = "LOG11223",
                Status = EquipmentStatus.Dostępny,
                Location = "K 108",
                Specification = "Bezprzewodowa, Podświetlana"
            },
            
        };
        return equipment;
    }
}