namespace Users.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<Role> Roles { get; set; } = [];
}