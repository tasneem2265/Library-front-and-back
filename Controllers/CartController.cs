// Controllers/CartController.cs
[Authorize]
public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

        if (cart == null)
        {
            cart = new Cart { ApplicationUserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var cart = await GetOrCreateCartAsync(userId);
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int bookId, int quantity = 1)
    {
        var userId = _userManager.GetUserId(User);
        var book = await _context.Books.FindAsync(bookId);

        if (book == null || book.StockQuantity < quantity)
        {
            TempData["Error"] = "الكمية المطلوبة غير متوفرة.";
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        var cart = await GetOrCreateCartAsync(userId);
        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.BookId == bookId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _context.CartItems.Add(new CartItem { CartId = cart.Id, BookId = bookId, Quantity = quantity });
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "تمت إضافة الكتاب للسلة.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);
        if (item != null)
        {
            if (quantity <= 0)
                _context.CartItems.Remove(item);
            else
                item.Quantity = quantity;

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var item = await _context.CartItems.FindAsync(cartItemId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        TempData["Success"] = "تم حذف الكتاب من السلة.";
        return RedirectToAction("Index");
    }
}