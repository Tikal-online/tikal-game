namespace Users.Domain.Entities;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; }

    public User(int id, string username)
    {
        Id = id;
        Username = username;
    }

    public User(string username)
    {
        Username = username;
    }
}