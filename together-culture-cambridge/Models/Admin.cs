using System.ComponentModel.DataAnnotations;

namespace together_culture_cambridge.Models;

public class Admin
{
    public int Id { get; set; }
    [StringLength(50)]public string FirstName { get; set; } = null!;
    [StringLength(50)]public string LastName { get; set; } = null!;
    [StringLength(200)]public string Email { get; set; } = null!;
    [StringLength(50)]public string? Phone { get; set; }
    [StringLength(100)]public string Password { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}