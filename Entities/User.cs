namespace Wypożyczlania_sprzętu.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public virtual Role Role { get; set; }
    
    public virtual List<Reservation> Reservations { get; set; }
    public virtual List<Borrowing> Borrowings { get; set; }
    public virtual List<FaultReport> FaultReports { get; set; }
    public IEnumerable<NotificationLog>? NotificationLogs { get; set; }
    
}