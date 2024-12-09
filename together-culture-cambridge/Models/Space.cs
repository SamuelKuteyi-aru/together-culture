using System.ComponentModel.DataAnnotations;

namespace together_culture_cambridge.Models;

public class Space
{
    public int Id { get; set; }
    public Membership.MembershipEnum MinimumAccessLevel { get; set; }
    [StringLength(20)] public string RoomId { get; set; } = null!;
    public int TotalSeats { get; set; }
    public DateTime OpeningTime { get; set; }
    public DateTime ClosingTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}