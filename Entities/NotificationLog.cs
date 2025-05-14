namespace Wypożyczlania_sprzętu.Entities;

public class NotificationLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime NotificationDate { get; set; } = DateTime.UtcNow;
    public string Message { get; set; }
    public string NotificationType { get; set; }
    public string Status { get; set; } 
    public string ErrorMessage { get; set; }
}