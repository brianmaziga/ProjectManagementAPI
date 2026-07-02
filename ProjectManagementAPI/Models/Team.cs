namespace ProjectManagementAPI.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int CreatedBy { get; set; }
}

public class TeamMember
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? UserEmail { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class AddTeamMemberDto
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
}