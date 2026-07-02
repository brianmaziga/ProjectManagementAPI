namespace ProjectManagementAPI.Models;

public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public string? TaskTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationDto
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int TaskId { get; set; }
}