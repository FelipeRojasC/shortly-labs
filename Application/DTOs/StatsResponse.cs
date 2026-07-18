namespace Shortly.Application.DTOs;

public class StatsResponse
{
    public int TotalLinks { get; init; }
    public int TotalClicks { get; init; }
    public List<LinkResponse> TopLinks { get; init; } = new();
}