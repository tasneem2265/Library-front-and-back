// Data/ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Many-to-Many: Book <-> Category
        builder.Entity<BookCategory>()
            .HasKey(bc => new { bc.BookId, bc.CategoryId });

        builder.Entity<BookCategory>()
            .HasOne(bc => bc.Book)
            .WithMany(b => b.BookCategories)
            .HasForeignKey(bc => bc.BookId);

        builder.Entity<BookCategory>()
            .HasOne(bc => bc.Category)
            .WithMany(c => c.BookCategories)
            .HasForeignKey(bc => bc.CategoryId);

        // Order -> OrderItems restrict delete
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Book)
            .WithMany(b => b.OrderItems)
            .HasForeignKey(oi => oi.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .Property(b => b.Price)
            .HasColumnType("decimal(10,2)");

        builder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(10,2)");

        builder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(10,2)");
    }
}