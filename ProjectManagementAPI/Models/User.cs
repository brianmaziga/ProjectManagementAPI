namespace ProjectManagementAPI.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Status { get; set; } = "active";

    public string Role { get; set; } = "member";

    public DateTime CreatedAt { get; set; }
}


public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "member";
}


public class BulkUserActionDto
{
    public List<int> UserIds { get; set; } = new();

    public string Action { get; set; } = string.Empty;
}