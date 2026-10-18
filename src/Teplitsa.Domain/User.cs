namespace Teplitsa.Domain;

public class User
{
    public int Id { get; private set; }
    public string Login { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public UserRole Role { get; set; }
}