// Models/Category.cs
public class Category
{
    public int Id { get; set; }
    
    [Required, StringLength(100)]
    public string Name { get; set; }
    
    public string Description { get; set; }

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}