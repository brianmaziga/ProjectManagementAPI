namespace ProjectManagementAPI.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OwnerId { get; set; }
}