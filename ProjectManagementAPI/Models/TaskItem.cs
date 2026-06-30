namespace ProjectManagementAPI.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "todo";
    public string Priority { get; set; } = "medium";
    public int ProjectId { get; set; }
    public int AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public int AssignedTo { get; set; }
    public DateTime? DueDate { get; set; }
}

public class UpdateTaskStatusDto
{
    public string Status { get; set; } = string.Empty;
}