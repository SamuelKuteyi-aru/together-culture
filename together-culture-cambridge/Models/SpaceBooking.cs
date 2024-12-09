namespace together_culture_cambridge.Models;

public class SpaceBooking
{
    public int Id { get; set; }
    public EndUser EndUser { get; set; } = null!;
    public int EndUserId { get; set; }
    public Space Space { get; set; } = null!;
    public int SpaceId { get; set; }
    public DateTime BookingDate { get; set; }
}
