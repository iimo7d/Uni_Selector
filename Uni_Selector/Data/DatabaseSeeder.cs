using Microsoft.EntityFrameworkCore;

namespace Uni_Selector.Data
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _db;
        private readonly IdentitySeeder _identitySeeder;
        private readonly AppDataSeeder _appDataSeeder;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            AppDbContext db,
            IdentitySeeder identitySeeder,
            AppDataSeeder appDataSeeder,
            ILogger<DatabaseSeeder> logger)
        {
            _db = db;
            _identitySeeder = identitySeeder;
            _appDataSeeder = appDataSeeder;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken ct = default)
        {
            // Make sure schema exists
            await _db.Database.MigrateAsync(ct);

            // Order matters:
            // 1) Identity (Roles/Users) -> so Students FK UserId works
            await _identitySeeder.SeedAsync(ct);

            // 2) Domain data
            await _appDataSeeder.SeedAsync(ct);
        }
    }
}
