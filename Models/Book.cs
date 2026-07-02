// Models/Book.cs
public class Book
{
    public int Id { get; set; }
    
    [Required, StringLength(200)]
    public string Title { get; set; }
    
    [Required, StringLength(100)]
    public string Author { get; set; }
    
    [StringLength(2000)]
    public string Description { get; set; }
    
    [Required]
    [Range(0, 100000)]
    public decimal Price { get; set; }
    
    public int StockQuantity { get; set; }
    
    public string CoverImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public bool IsAvailable { get; set; } = true;

    // Many-to-Many with Category
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
    
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}