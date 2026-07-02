// Models/OrderItem.cs
public class OrderItem
{
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    public Order Order { get; set; }
    
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    public int Quantity { get; set; }
    
    // سعر وقت الطلب (عشان لو السعر اتغير بعدين)
    public decimal UnitPrice { get; set; }
}