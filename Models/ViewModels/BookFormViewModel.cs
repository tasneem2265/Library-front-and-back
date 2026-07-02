// Models/ViewModels/BookFormViewModel.cs
public class BookFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "العنوان مطلوب")]
    [StringLength(200)]
    public string Title { get; set; }

    [Required(ErrorMessage = "اسم المؤلف مطلوب")]
    [StringLength(100)]
    public string Author { get; set; }

    [StringLength(2000)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 100000, ErrorMessage = "السعر غير صحيح")]
    public decimal Price { get; set; }

    [Range(0, 10000)]
    public int StockQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

    public IFormFile CoverImage { get; set; }
    public string ExistingImageUrl { get; set; }

    public List<int> SelectedCategoryIds { get; set; } = new();
}