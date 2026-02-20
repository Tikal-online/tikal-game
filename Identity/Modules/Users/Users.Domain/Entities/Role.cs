namespace Users.Domain.Entities;

public class Role
{
    public string Name { get; }

    public Role(string name)
    {
        Name = name;
    }
}