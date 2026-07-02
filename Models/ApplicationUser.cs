// Models/ApplicationUser.cs
public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string FullName { get; set; }
    
    public string Address { get; set; }
    
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public Cart Cart { get; set; }
}