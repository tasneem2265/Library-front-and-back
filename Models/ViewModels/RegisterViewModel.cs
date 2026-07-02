// Models/ViewModels/RegisterViewModel.cs
public class RegisterViewModel
{
    [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
    [EmailAddress(ErrorMessage = "بريد إلكتروني غير صحيح")]
    public string Email { get; set; }

    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [Phone]
    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "كلمة المرور غير متطابقة")]
    public string ConfirmPassword { get; set; }
}