// Controllers/HomeController.cs
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var latestBooks = await _context.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Where(b => b.IsAvailable)
            .OrderByDescending(b => b.CreatedAt)
            .Take(8)
            .ToListAsync();

        var categories = await _context.Categories.ToListAsync();

        ViewBag.Categories = categories;
        return View(latestBooks);
    }
}