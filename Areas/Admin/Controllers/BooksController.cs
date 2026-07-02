// Areas/Admin/Controllers/BooksController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public BooksController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return View(books);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        var book = new Book
        {
            Title = model.Title,
            Author = model.Author,
            Description = model.Description,
            Price = model.Price,
            StockQuantity = model.StockQuantity,
            IsAvailable = model.IsAvailable
        };

        if (model.CoverImage != null)
        {
            book.CoverImageUrl = await SaveImageAsync(model.CoverImage);
        }

        foreach (var catId in model.SelectedCategoryIds ?? new List<int>())
        {
            book.BookCategories.Add(new BookCategory { CategoryId = catId });
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم إضافة الكتاب بنجاح.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null) return NotFound();

        var model = new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            IsAvailable = book.IsAvailable,
            ExistingImageUrl = book.CoverImageUrl,
            SelectedCategoryIds = book.BookCategories.Select(bc => bc.CategoryId).ToList()
        };

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }

        var book = await _context.Books
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == model.Id);

        if (book == null) return NotFound();

        book.Title = model.Title;
        book.Author = model.Author;
        book.Description = model.Description;
        book.Price = model.Price;
        book.StockQuantity = model.StockQuantity;
        book.IsAvailable = model.IsAvailable;

        if (model.CoverImage != null)
        {
            book.CoverImageUrl = await SaveImageAsync(model.CoverImage);
        }

        // تحديث الأقسام
        book.BookCategories.Clear();
        foreach (var catId in model.SelectedCategoryIds ?? new List<int>())
        {
            book.BookCategories.Add(new BookCategory { BookId = book.Id, CategoryId = catId });
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "تم تعديل الكتاب بنجاح.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            TempData["Success"] = "تم حذف الكتاب.";
        }
        return RedirectToAction("Index");
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "books");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/images/books/{fileName}";
    }
}