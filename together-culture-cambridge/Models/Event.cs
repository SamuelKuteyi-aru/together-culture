using System.ComponentModel.DataAnnotations;

namespace together_culture_cambridge.Models;

public class Event
{
    public int Id { get; set; }
    [StringLength(150)] public string Name { get; set; } = null!;
    [StringLength(350)]public string Address { get; set; } = null!;
    [StringLength(1000)] public string Description { get; set; } = null!;
    public int TotalSpaces { get; set; } 
    public double TicketPrice { get; set; } 
    public DateTime StartTime { get; set; } 
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}