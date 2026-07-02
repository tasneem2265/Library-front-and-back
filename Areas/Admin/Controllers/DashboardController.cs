// Areas/Admin/Controllers/DashboardController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBooks = await _context.Books.CountAsync();
        ViewBag.TotalOrders = await _context.Orders.CountAsync();
        ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
        ViewBag.TotalCustomers = await _context.Users.CountAsync();
        ViewBag.RecentOrders = await _context.Orders
            .Include(o => o.ApplicationUser)
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .ToListAsync();

        return View();
    }
}