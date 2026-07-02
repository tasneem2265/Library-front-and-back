// Controllers/BooksController.cs
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId, string search, decimal? minPrice, decimal? maxPrice, int page = 1)
    {
        const int pageSize = 12;

        var query = _context.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Where(b => b.IsAvailable)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId.Value));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));

        if (minPrice.HasValue)
            query = query.Where(b => b.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(b => b.Price <= maxPrice.Value);

        var totalBooks = await query.CountAsync();

        var books = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.CurrentCategory = categoryId;
        ViewBag.Search = search;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();

        return View(book);
    }
}