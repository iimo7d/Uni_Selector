using Microsoft.AspNetCore.Identity;
using Uni_Selector.Models;

namespace Uni_Selector.Data
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentitySeeder> _logger;

        public IdentitySeeder(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<IdentitySeeder> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public static readonly DateTime SeedNow =
            new DateTime(2025, 12, 01, 0, 0, 0, DateTimeKind.Utc);

        public async Task SeedAsync(CancellationToken ct = default)
        {
            await SeedRolesAsync(ct);
            await SeedUsersAsync(ct);
        }

        private async Task SeedRolesAsync(CancellationToken ct)
        {
            var roles = new[]
            {
                "PlatformAdmin",
                "Student",
                "UniversityRep",
                "BtecAuthority"
            };

            foreach (var roleName in roles)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                    continue;

                var role = new IdentityRole(roleName)
                {
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var res = await _roleManager.CreateAsync(role);
                if (!res.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': {string.Join(", ", res.Errors.Select(e => e.Description))}");
            }
        }

        private async Task SeedUsersAsync(CancellationToken ct)
        {
            // Helper: Create user ONLY if Id not exists.
            async Task<ApplicationUser> EnsureUserAsync(ApplicationUser u, string password)
            {
                var byId = await _userManager.FindByIdAsync(u.Id);
                if (byId != null)
                    return byId;

                // If email exists with different Id => this DB already has conflicting data.
                var byEmail = await _userManager.FindByEmailAsync(u.Email!);
                if (byEmail != null && byEmail.Id != u.Id)
                {
                    throw new InvalidOperationException(
                        $"User email '{u.Email}' already exists with Id '{byEmail.Id}', but seeder wants Id '{u.Id}'. " +
                        $"Either drop DB or change seed IDs/emails.");
                }

                var res = await _userManager.CreateAsync(u, password);
                if (!res.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create user '{u.Email}': {string.Join(", ", res.Errors.Select(e => e.Description))}");

                return u;
            }

            // Users
            var adminUser = await EnsureUserAsync(new ApplicationUser
            {
                Id = "admin-001",
                UserName = "admin@uni-selector.jo",
                Email = "admin@uni-selector.jo",
                EmailConfirmed = true,
                FullName = "Platform Administrator",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "ADMIN-SEC-0001"
            }, "Admin@123");

            var btecUser = await EnsureUserAsync(new ApplicationUser
            {
                Id = "btec-001",
                UserName = "btec@education.gov.jo",
                Email = "btec@education.gov.jo",
                EmailConfirmed = true,
                FullName = "BTEC Authority Officer",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "BTEC-SEC-0001"
            }, "Btec@123");

            var germanRepUser = await EnsureUserAsync(new ApplicationUser
            {
                Id = "rep-001",
                UserName = "rep@gju.edu.jo",
                Email = "rep@gju.edu.jo",
                EmailConfirmed = true,
                FullName = "Ahmad Al-Hassan",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "REP-001-SEC-0001"
            }, "Rep@1234");

            var meuRepUser = await EnsureUserAsync(new ApplicationUser
            {
                Id = "rep-002",
                UserName = "rep@meu.edu.jo",
                Email = "rep@meu.edu.jo",
                EmailConfirmed = true,
                FullName = "Sarah Al-Ahmad",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "REP-002-SEC-0001"
            }, "Rep@1234");

            var student1User = await EnsureUserAsync(new ApplicationUser
            {
                Id = "student-001",
                UserName = "mohammad.khaled@example.com",
                Email = "mohammad.khaled@example.com",
                EmailConfirmed = true,
                FullName = "Mohammad Khaled",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "STUDENT-001-SEC-0001"
            }, "Student@123");

            var student2User = await EnsureUserAsync(new ApplicationUser
            {
                Id = "student-002",
                UserName = "fatima.hassan@example.com",
                Email = "fatima.hassan@example.com",
                EmailConfirmed = true,
                FullName = "Fatima Hassan",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "STUDENT-002-SEC-0001"
            }, "Student@123");

            var student3User = await EnsureUserAsync(new ApplicationUser
            {
                Id = "student-003",
                UserName = "omar.salem@example.com",
                Email = "omar.salem@example.com",
                EmailConfirmed = true,
                FullName = "Omar Salem",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "STUDENT-003-SEC-0001"
            }, "Student@123");

            var student4User = await EnsureUserAsync(new ApplicationUser
            {
                Id = "student-004",
                UserName = "leen.ibrahim@example.com",
                Email = "leen.ibrahim@example.com",
                EmailConfirmed = true,
                FullName = "Leen Ibrahim",
                IsActive = true,
                CreatedAt = SeedNow,
                SecurityStamp = "STUDENT-004-SEC-0001"
            }, "Student@123");

            // Role assignments (by role NAME — no dependency on role Ids)
            await EnsureUserInRole(adminUser, "PlatformAdmin");
            await EnsureUserInRole(btecUser, "BtecAuthority");

            await EnsureUserInRole(germanRepUser, "UniversityRep");
            await EnsureUserInRole(meuRepUser, "UniversityRep");

            await EnsureUserInRole(student1User, "Student");
            await EnsureUserInRole(student2User, "Student");
            await EnsureUserInRole(student3User, "Student");
            await EnsureUserInRole(student4User, "Student");
        }

        private async Task EnsureUserInRole(ApplicationUser user, string roleName)
        {
            if (await _userManager.IsInRoleAsync(user, roleName))
                return;

            var res = await _userManager.AddToRoleAsync(user, roleName);
            if (!res.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to add '{user.Email}' to role '{roleName}': {string.Join(", ", res.Errors.Select(e => e.Description))}");
        }
    }
}
