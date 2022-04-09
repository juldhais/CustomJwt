namespace CustomJwt.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public Role Role { get; set; }
    public int? RoleId { get; set; }
}