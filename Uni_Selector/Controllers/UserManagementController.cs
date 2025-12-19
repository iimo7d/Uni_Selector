using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Uni_Selector.Data;
using Uni_Selector.Models;
using Uni_Selector.ViewModels.UserManagement;

namespace Uni_Selector.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = "", string roleFilter = "", string statusFilter = "", int page = 1, int pageSize = 10)
        {
            // Start with all users
            var query = _userManager.Users.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                bool isActive = statusFilter.ToLower() == "active";
                query = query.Where(u => u.IsActive == isActive);
            }

            // Get all users for role filtering (need to do this after getting roles)
            var allUsers = await query
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            // Filter by role if specified
            List<ApplicationUser> filteredUsers;
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                filteredUsers = new List<ApplicationUser>();
                foreach (var user in allUsers)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (userRoles.Contains(roleFilter))
                    {
                        filteredUsers.Add(user);
                    }
                }
            }
            else
            {
                filteredUsers = allUsers;
            }

            // Calculate pagination
            var totalUsers = filteredUsers.Count;
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            var pagedUsers = filteredUsers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create view models
            var userViewModels = new List<UserListViewModel>();
            foreach (var user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    Roles = string.Join(", ", roles)
                });
            }

            // Get all roles for filter dropdown
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            // Create pagination model
            var model = new UserListPageViewModel
            {
                Users = userViewModels,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalUsers = totalUsers,
                SearchTerm = searchTerm,
                RoleFilter = roleFilter,
                StatusFilter = statusFilter,
                AvailableRoles = allRoles,
                HasPreviousPage = page > 1,
                HasNextPage = page < totalPages
            };

            return View(model);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Roles = roles.ToList()
            };

            // Load University Rep info if applicable
            if (roles.Contains(UserRoles.UniversityRep))
            {
                var rep = await _context.UniversityRepresentatives
                    .Include(r => r.University)
                    .FirstOrDefaultAsync(r => r.UserId == id);
                model.UniversityRepresentative = rep;
            }

            return View(model);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateUserViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            // Create the user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await PopulateDropdowns(model);
                return View(model);
            }

            // Assign role
            if (!string.IsNullOrEmpty(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            // Create University Representative record if needed
            if (model.Role == UserRoles.UniversityRep && model.UniversityId.HasValue)
            {
                var rep = new UniversityRepresentative
                {
                    UserId = user.Id,
                    UniversityId = model.UniversityId.Value,
                    Position = model.Position,
                    CanManagePrograms = model.CanManagePrograms,
                    CanManageFees = model.CanManageFees,
                    CanViewApplications = model.CanViewApplications,
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.UniversityRepresentatives.Add(rep);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"User '{user.FullName}' created successfully with role '{model.Role}'.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/{id}/Edit - Edit user form
        [HttpGet("{id}/Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Role = roles.FirstOrDefault()
            };

            // Load University Rep info if applicable
            if (model.Role == UserRoles.UniversityRep)
            {
                var rep = await _context.UniversityRepresentatives
                    .FirstOrDefaultAsync(r => r.UserId == id);
                if (rep != null)
                {
                    model.UniversityId = rep.UniversityId;
                    model.Position = rep.Position;
                    model.CanManagePrograms = rep.CanManagePrograms;
                    model.CanManageFees = rep.CanManageFees;
                    model.CanViewApplications = rep.CanViewApplications;
                }
            }

            await PopulateDropdowns(model);
            return View(model);
        }

        // POST: /Admin/Users/{id}/Edit - Update user
        [HttpPost("{id}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            // Update user basic info
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await PopulateDropdowns(model);
                return View(model);
            }

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            }

            // Handle role changes
            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            if (currentRole != model.Role && !string.IsNullOrEmpty(model.Role))
            {
                // Remove old role
                if (!string.IsNullOrEmpty(currentRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                // Add new role
                await _userManager.AddToRoleAsync(user, model.Role);

                // Handle UniversityRepresentative record based on role change
                var existingRep = await _context.UniversityRepresentatives
                    .FirstOrDefaultAsync(r => r.UserId == id);

                if (model.Role == UserRoles.UniversityRep)
                {
                    // Creating or updating University Rep
                    if (!model.UniversityId.HasValue)
                    {
                        ModelState.AddModelError("UniversityId", "University is required for University Representative role.");
                        await PopulateDropdowns(model);
                        return View(model);
                    }

                    if (existingRep == null)
                    {
                        // Create new UniversityRepresentative record
                        var rep = new UniversityRepresentative
                        {
                            UserId = user.Id,
                            UniversityId = model.UniversityId.Value,
                            Position = model.Position,
                            CanManagePrograms = model.CanManagePrograms,
                            CanManageFees = model.CanManageFees,
                            CanViewApplications = model.CanViewApplications,
                            AssignedAt = DateTime.UtcNow,
                            IsActive = user.IsActive
                        };
                        _context.UniversityRepresentatives.Add(rep);
                    }
                    else
                    {
                        // Update existing record
                        existingRep.UniversityId = model.UniversityId.Value;
                        existingRep.Position = model.Position;
                        existingRep.CanManagePrograms = model.CanManagePrograms;
                        existingRep.CanManageFees = model.CanManageFees;
                        existingRep.CanViewApplications = model.CanViewApplications;
                        existingRep.IsActive = user.IsActive;
                    }
                    await _context.SaveChangesAsync();
                }
                else if (existingRep != null)
                {
                    // Changed from UniversityRep to another role - remove the record
                    _context.UniversityRepresentatives.Remove(existingRep);
                    await _context.SaveChangesAsync();
                }
            }
            else if (model.Role == UserRoles.UniversityRep && model.UniversityId.HasValue)
            {
                // Role didn't change, but still UniversityRep - update info
                var rep = await _context.UniversityRepresentatives
                    .FirstOrDefaultAsync(r => r.UserId == id);
                if (rep != null)
                {
                    rep.UniversityId = model.UniversityId.Value;
                    rep.Position = model.Position;
                    rep.CanManagePrograms = model.CanManagePrograms;
                    rep.CanManageFees = model.CanManageFees;
                    rep.CanViewApplications = model.CanViewApplications;
                    rep.IsActive = user.IsActive;
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction(nameof(Details), new { id = user.Id });
        }

        [HttpPost("{id}/Lock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id, string returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            // Update related UniversityRepresentative if exists
            var rep = await _context.UniversityRepresentatives.FirstOrDefaultAsync(r => r.UserId == id);
            if (rep != null)
            {
                rep.IsActive = false;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "User account locked successfully.";

            // Return to Index with preserved filters
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("{id}/Unlock")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id, string returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            // Update related UniversityRepresentative if exists
            var rep = await _context.UniversityRepresentatives.FirstOrDefaultAsync(r => r.UserId == id);
            if (rep != null)
            {
                rep.IsActive = true;
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "User account unlocked successfully.";

            // Return to Index with preserved filters
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Export")]
        public async Task<IActionResult> Export()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Users");

            // Headers
            worksheet.Cell(1, 1).Value = "Full Name";
            worksheet.Cell(1, 2).Value = "Email";
            worksheet.Cell(1, 3).Value = "Phone Number";
            worksheet.Cell(1, 4).Value = "Roles";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "Created At";
            worksheet.Cell(1, 7).Value = "Last Login";

            // Style header
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            // Data
            int row = 2;
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                worksheet.Cell(row, 1).Value = user.FullName;
                worksheet.Cell(row, 2).Value = user.Email;
                worksheet.Cell(row, 3).Value = user.PhoneNumber ?? "";
                worksheet.Cell(row, 4).Value = string.Join(", ", roles);
                worksheet.Cell(row, 5).Value = user.IsActive ? "Active" : "Locked";
                worksheet.Cell(row, 6).Value = user.CreatedAt.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 7).Value = user.LastLoginAt?.ToString("yyyy-MM-dd") ?? "";
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0; // Reset stream position to beginning

            var fileName = $"Users_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
        #region Helper
        private async Task PopulateDropdowns(dynamic model)
        {
            // Get all roles
            var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            model.AvailableRoles = new SelectList(roles);

            // Get all universities for University Rep role
            var universities = await _context.Universities
                .Where(u => u.IsActive)
                .OrderBy(u => u.NameEnglish)
                .Select(u => new { u.Id, u.NameEnglish })
                .ToListAsync();
            model.AvailableUniversities = new SelectList(universities, "Id", "NameEnglish");
        }
        #endregion
    }
}
