namespace Shortly.Application.DTOs;

public class CreateLinkRequest
{
    public string Url { get; set; } = null!;
    public long? UserId { get; set; }
}