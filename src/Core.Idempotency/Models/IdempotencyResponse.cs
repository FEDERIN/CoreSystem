
namespace Core.Idempotency.Models;

public class IdempotencyResponse
{
    public int StatusCode { get; set; }
    public string? ContentType { get; set; }
    public string? Body { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}