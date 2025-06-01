namespace Wypożyczlania_sprzętu.Exceptions;

public class BadRequestException: Exception
{
    public BadRequestException(string message) : base(message)
    {
        
    }
}