using System.ComponentModel.DataAnnotations;

namespace together_culture_cambridge.Models;

public class Membership
{
    public enum MembershipEnum
    {
        Guest,
        Community,
        KeyAccess,
        CreativeWorkspace
    }
    public int Id { get; set; }
    [StringLength(20)]public string? Name { get; set; }
    public MembershipEnum MembershipType { get; set; }
    public double MonthlyPrice { get; set; }
    public double JoiningFee { get; set; }
}