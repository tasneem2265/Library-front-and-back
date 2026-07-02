// Controllers/OrdersController.cs
[Authorize]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Checkout()
    {
        var userId = _userManager.GetUserId(User);
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "سلتك فارغة.";
            return RedirectToAction("Index", "Cart");
        }

        var user = await _userManager.FindByIdAsync(userId);
        var model = new CheckoutViewModel
        {
            CartItems = cart.CartItems.ToList(),
            Total = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity),
            ShippingAddress = user.Address,
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        var userId = _userManager.GetUserId(User);
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (cart == null || !cart.CartItems.Any())
        {
            TempData["Error"] = "سلتك فارغة.";
            return RedirectToAction("Index", "Cart");
        }

        if (string.IsNullOrWhiteSpace(model.ShippingAddress) || string.IsNullOrWhiteSpace(model.PhoneNumber))
        {
            TempData["Error"] = "من فضلك أدخلي العنوان ورقم الهاتف.";
            return RedirectToAction("Checkout");
        }

        // تأكد من توفر الكمية قبل تأكيد الطلب
        foreach (var item in cart.CartItems)
        {
            if (item.Book.StockQuantity < item.Quantity)
            {
                TempData["Error"] = $"الكمية المتاحة من \"{item.Book.Title}\" غير كافية.";
                return RedirectToAction("Index", "Cart");
            }
        }

        var order = new Order
        {
            ApplicationUserId = userId,
            ShippingAddress = model.ShippingAddress,
            PhoneNumber = model.PhoneNumber,
            Status = OrderStatus.Pending,
            TotalAmount = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity),
            OrderItems = cart.CartItems.Select(ci => new OrderItem
            {
                BookId = ci.BookId,
                Quantity = ci.Quantity,
                UnitPrice = ci.Book.Price
            }).ToList()
        };

        // خصم الكمية من المخزون
        foreach (var item in cart.CartItems)
        {
            item.Book.StockQuantity -= item.Quantity;
        }

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync();

        TempData["Success"] = "تم تأكيد طلبك بنجاح!";
        return RedirectToAction("MyOrders");
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = _userManager.GetUserId(User);
        var orders = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
            .Where(o => o.ApplicationUserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = _userManager.GetUserId(User);
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == id && o.ApplicationUserId == userId);

        if (order == null) return NotFound();

        return View(order);
    }
}