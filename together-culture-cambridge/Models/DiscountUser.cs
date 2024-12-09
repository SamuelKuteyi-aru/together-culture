namespace together_culture_cambridge.Models;

public class DiscountUser
{
    public int Id { get; set; }
    public int DiscountId { get; set; }
    public Discount Discount { get; set; } = null!;
    public int EndUserId { get; set; }
    public EndUser EndUser { get; set; } = null!;
}