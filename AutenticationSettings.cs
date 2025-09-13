namespace Wypożyczlania_sprzętu;

public class AutenticationSettings
{
    public string JwtKey { get; set; }
    public int JwtExpireDays { get; set; }
    public string JwtIssuer { get; set; }
    public string JwtAudience { get; set; }
}