namespace together_culture_cambridge.Models;

public class EventBooking
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public int EndUserId { get; set; }
    public EndUser EndUser { get; set; } = null!;
}