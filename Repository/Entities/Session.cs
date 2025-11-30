using Repository.Entities.Enums;

namespace Repository.Entities;

public class Session
{
    public required string SessionId { get; set; }
    public required string PlayerId { get; set; }
    public  required string GameId { get; set; }
    public required DateTime StartedAt { get; set; }
    public required SessionStatus Status { get; set; }
}