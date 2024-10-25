using System.ComponentModel.DataAnnotations;

namespace together_culture_cambridge.Models;

public class EndUser
{
    public enum GenderEnum
    {
        Male,
        Female,
        Other
    }
    
    public int Id { get; set; }

    public int MembershipId { get; set; }
    public Membership Membership { get; set; } = null!;
    
    [StringLength(50)]public string? FirstName { get; set;  }
    [StringLength(50)] public string? LastName { get; set; }
    [StringLength(250)] public string? Email { get; set; }
    [StringLength(200)]public string? Password { get; set; }
    [StringLength(50)]public string? Phone { get; set; }
    public GenderEnum Gender { get; set; }
    public DateTime DateOfBirth { get; set;  }
    public DateTime? SubscriptionDate { get; set;  }
    public DateTime? CheckIn {  get; set;  }
    public DateTime? CheckOut { get; set;  }
    public DateTime CreatedAt { get; set;  }
    public DateTime UpdatedAt { get; set;  }
}