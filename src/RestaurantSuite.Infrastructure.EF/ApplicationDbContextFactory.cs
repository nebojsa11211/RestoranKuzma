using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RestaurantSuite.Infrastructure.EF;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Use SQLite for local development
        optionsBuilder.UseSqlite("Data Source=restorankuzma.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
