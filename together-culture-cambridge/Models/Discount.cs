using System.ComponentModel.DataAnnotations;

namespace together_culture_cambridge.Models;

public class Discount
{
    public int Id { get; set; }
    [StringLength(50)] public string Code { get; set; } = null!;
    public double Percentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpirationDate { get; set; }
}