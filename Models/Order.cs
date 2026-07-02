// Models/Order.cs
public enum OrderStatus
{
    Pending,        // قيد التجهيز
    Shipped,        // تم الشحن
    Delivered,      // تم التسليم
    Cancelled       // ملغي
}

public class Order
{
    public int Id { get; set; }
    
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    [Required, StringLength(300)]
    public string ShippingAddress { get; set; }
    
    [Required, StringLength(20)]
    public string PhoneNumber { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}