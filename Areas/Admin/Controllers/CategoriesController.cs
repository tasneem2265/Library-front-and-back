// Areas/Admin/Controllers/CategoriesController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.BookCategories)
            .ToListAsync();

        return View(categories);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Category model)
    {
        if (!ModelState.IsValid) return View(model);

        _context.Categories.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = "تم إضافة القسم بنجاح.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Category model)
    {
        if (!ModelState.IsValid) return View(model);

        var category = await _context.Categories.FindAsync(model.Id);
        if (category == null) return NotFound();

        category.Name = model.Name;
        category.Description = model.Description;

        await _context.SaveChangesAsync();
        TempData["Success"] = "تم تعديل القسم بنجاح.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories
            .Include(c => c.BookCategories)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();

        if (category.BookCategories.Any())
        {
            TempData["Error"] = "لا يمكن حذف قسم مرتبط بكتب. احذفي الكتب أو غيّري تصنيفها أولاً.";
            return RedirectToAction("Index");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "تم حذف القسم.";
        return RedirectToAction("Index");
    }
}