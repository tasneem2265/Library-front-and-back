// Models/ViewModels/CheckoutViewModel.cs
public class CheckoutViewModel
{
    public List<CartItem> CartItems { get; set; } = new();
    public decimal Total { get; set; }

    [Required(ErrorMessage = "من فضلك أدخلي عنوان الشحن")]
    [StringLength(300)]
    public string ShippingAddress { get; set; }

    [Required(ErrorMessage = "من فضلك أدخلي رقم الهاتف")]
    [Phone]
    public string PhoneNumber { get; set; }
}