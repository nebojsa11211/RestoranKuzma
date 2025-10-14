using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Infrastructure.EF;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants { get; set; } = null!;
    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<MenuItemIngredient> MenuItemIngredients { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<OrderItemCustomization> OrderItemCustomizations { get; set; } = null!;
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
    public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
    public DbSet<Recipe> Recipes { get; set; } = null!;
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; } = null!;
    public DbSet<StockTransaction> StockTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Restaurant configuration - singleton entity with simple properties only
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Timezone).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.SettingsJson).HasColumnType("text");
        });

        // MenuItem configuration - removed RestaurantId FK, updated index
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Ingredients)
                .WithOne(e => e.MenuItem)
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.CategoryId, e.IsAvailable });
        });

        // MenuItemIngredient configuration
        modelBuilder.Entity<MenuItemIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IngredientName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.QuantityInGrams).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => new { e.MenuItemId, e.DisplayOrder });
        });

        // Category configuration - removed RestaurantId FK, updated index
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity.HasIndex(e => e.DisplayOrder);
        });

        // Order configuration - removed RestaurantId FK, updated index
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.Table)
                .WithMany()
                .HasForeignKey(e => e.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Waiter)
                .WithMany()
                .HasForeignKey(e => e.WaiterId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Guest)
                .WithMany()
                .HasForeignKey(e => e.GuestId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasMany(e => e.OrderItems)
                .WithOne(e => e.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.Status, e.CreatedAt });
        });

        // OrderItem configuration - unchanged, no RestaurantId
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SpecialInstructions).HasMaxLength(500);

            entity.HasOne(e => e.MenuItem)
                .WithMany()
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Customizations)
                .WithOne(e => e.OrderItem)
                .HasForeignKey(e => e.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItemCustomization configuration
        modelBuilder.Entity<OrderItemCustomization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IngredientName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(200);

            entity.HasIndex(e => e.OrderItemId);
        });

        // Table configuration - removed RestaurantId FK, updated unique index
        // Also ignoring CreatedAt and UpdatedAt columns that don't exist in the database
        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TableNumber).IsRequired().HasMaxLength(50);
            
            // Ignore properties that don't exist in the database
            entity.Ignore(e => e.CreatedAt);
            entity.Ignore(e => e.UpdatedAt);

            entity.HasIndex(e => e.TableNumber).IsUnique();
        });

        // User configuration - removed RestaurantId FK, updated indexes
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.RefreshToken).HasMaxLength(500);

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.RefreshToken);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TipAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TransactionId).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.OrderId, e.CreatedAt });
            entity.HasIndex(e => e.Status);
        });

        // InventoryItem configuration
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CurrentStock).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MinimumStockLevel).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // Recipe configuration
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.MenuItem)
                .WithMany()
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Ingredients)
                .WithOne(e => e.Recipe)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.MenuItemId).IsUnique();
            entity.HasIndex(e => e.IsActive);
        });

        // RecipeIngredient configuration
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantityRequired).HasColumnType("decimal(18,2)");

            entity.HasOne(e => e.InventoryItem)
                .WithMany()
                .HasForeignKey(e => e.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.RecipeId, e.InventoryItemId }).IsUnique();
        });

        // StockTransaction configuration
        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PreviousStock).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NewStock).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.InventoryItem)
                .WithMany()
                .HasForeignKey(e => e.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.InventoryItemId, e.CreatedAt });
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.TransactionType);
        });

        // Apply Invoice configurations
        modelBuilder.ApplyConfiguration(new Configurations.InvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.InvoiceItemConfiguration());
    }
}
