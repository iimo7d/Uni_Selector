using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Uni_Selector.Data;
using Uni_Selector.Hubs;
using Uni_Selector.Models;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Controllers
{
    [Authorize(Roles = UserRoles.PlatformAdmin)]
    public class UniversityAdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<UniversityAdminController> _logger;

        public UniversityAdminController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext,
            ILogger<UniversityAdminController> logger)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, string? province, bool? isActive, int page = 1, int pageSize = 20)
        {
            try
            {
                var query = _context.Universities
                    .Include(u => u.Representatives)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(u =>
                        u.NameEnglish.Contains(searchTerm) ||
                        u.NameArabic.Contains(searchTerm) ||
                        u.City.Contains(searchTerm));
                }

                if (!string.IsNullOrWhiteSpace(province))
                {
                    query = query.Where(u => u.Province == province);
                }

                if (isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == isActive.Value);
                }

                var totalUniversities = await query.CountAsync();

                var universities = await query
                    .OrderByDescending(u => u.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.SearchTerm = searchTerm;
                ViewBag.Province = province;
                ViewBag.IsActive = isActive;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling(totalUniversities / (double)pageSize);
                ViewBag.TotalUniversities = totalUniversities;

                ViewBag.Provinces = await _context.Universities
                    .Select(u => u.Province)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToListAsync();

                return View(universities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading universities list");
                TempData["Error"] = "An error occurred while loading universities.";
                return View(new List<University>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                // Prepare dropdown data
                ViewBag.CommissionModes = new SelectList(Enum.GetValues(typeof(CommissionMode)));
                ViewBag.UniversityTypes = new SelectList(Enum.GetValues(typeof(UniversityType)));

                return View(new University());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create university form");
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(University university, IFormFile? logo, IFormFile? coverImage)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.CommissionModes = new SelectList(Enum.GetValues(typeof(CommissionMode)));
                    ViewBag.UniversityTypes = new SelectList(Enum.GetValues(typeof(UniversityType)));
                    return View(university);
                }

                if (logo != null && logo.Length > 0)
                {
                    var logoFileName = $"{Guid.NewGuid()}_{Path.GetFileName(logo.FileName)}";
                    var logoPath = Path.Combine("wwwroot", "uploads", "universities", "logos", logoFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(logoPath)!);

                    using (var stream = new FileStream(logoPath, FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }

                    university.LogoPath = $"/uploads/universities/logos/{logoFileName}";
                }

                if (coverImage != null && coverImage.Length > 0)
                {
                    var imageFileName = $"{Guid.NewGuid()}_{Path.GetFileName(coverImage.FileName)}";
                    var imagePath = Path.Combine("wwwroot", "uploads", "universities", "images", imageFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(stream);
                    }

                    university.ImagePath = $"/uploads/universities/images/{imageFileName}";
                }

                university.CreatedAt = DateTime.UtcNow;
                university.IsActive = true;

                _context.Universities.Add(university);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"University created: {university.NameEnglish} (ID: {university.Id}) by admin {User.Identity?.Name}");

                TempData["Success"] = $"University '{university.NameEnglish}' has been created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating university");
                ModelState.AddModelError("", "An error occurred while creating the university.");
                ViewBag.CommissionModes = new SelectList(Enum.GetValues(typeof(CommissionMode)));
                ViewBag.UniversityTypes = new SelectList(Enum.GetValues(typeof(UniversityType)));
                return View(university);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var university = await _context.Universities
                    .Include(u => u.Representatives)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (university == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Prepare dropdown data
                ViewBag.CommissionModes = new SelectList(Enum.GetValues(typeof(CommissionMode)), university.CommissionMode);
                ViewBag.UniversityTypes = new SelectList(Enum.GetValues(typeof(UniversityType)), university.Type);

                return View(university);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading edit form for university ID: {id}");
                TempData["Error"] = "An error occurred while loading the university details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, University university, IFormFile? logo, IFormFile? coverImage)
        {
            try
            {
                if (id != university.Id)
                {
                    TempData["Error"] = "Invalid university ID.";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.CommissionModes = new SelectList(Enum.GetValues(typeof(CommissionMode)), university.CommissionMode);
                    ViewBag.UniversityTypes = new SelectList(Enum.GetValues(typeof(UniversityType)), university.Type);
                    return View(university);
                }

                var existingUniversity = await _context.Universities.FindAsync(id);
                if (existingUniversity == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                existingUniversity.NameArabic = university.NameArabic;
                existingUniversity.NameEnglish = university.NameEnglish;
                existingUniversity.Type = university.Type;
                existingUniversity.Province = university.Province;
                existingUniversity.City = university.City;
                existingUniversity.FullAddress = university.FullAddress;
                existingUniversity.Latitude = university.Latitude;
                existingUniversity.Longitude = university.Longitude;
                existingUniversity.AcademicAccreditation = university.AcademicAccreditation;
                existingUniversity.PhoneNumber = university.PhoneNumber;
                existingUniversity.Email = university.Email;
                existingUniversity.OfficialWebsite = university.OfficialWebsite;
                existingUniversity.CommissionMode = university.CommissionMode;
                existingUniversity.UpdatedAt = DateTime.UtcNow;

                if (logo != null && logo.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingUniversity.LogoPath))
                    {
                        var oldLogoPath = Path.Combine("wwwroot", existingUniversity.LogoPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldLogoPath))
                        {
                            System.IO.File.Delete(oldLogoPath);
                        }
                    }

                    var logoFileName = $"{Guid.NewGuid()}_{Path.GetFileName(logo.FileName)}";
                    var logoPath = Path.Combine("wwwroot", "uploads", "universities", "logos", logoFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(logoPath)!);

                    using (var stream = new FileStream(logoPath, FileMode.Create))
                    {
                        await logo.CopyToAsync(stream);
                    }

                    existingUniversity.LogoPath = $"/uploads/universities/logos/{logoFileName}";
                }

                if (coverImage != null && coverImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existingUniversity.ImagePath))
                    {
                        var oldImagePath = Path.Combine("wwwroot", existingUniversity.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var imageFileName = $"{Guid.NewGuid()}_{Path.GetFileName(coverImage.FileName)}";
                    var imagePath = Path.Combine("wwwroot", "uploads", "universities", "images", imageFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await coverImage.CopyToAsync(stream);
                    }

                    existingUniversity.ImagePath = $"/uploads/universities/images/{imageFileName}";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"University updated: {existingUniversity.NameEnglish} (ID: {id}) by admin {User.Identity?.Name}");

                TempData["Success"] = $"University '{existingUniversity.NameEnglish}' has been updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating university ID: {id}");
                ModelState.AddModelError("", "An error occurred while updating the university.");
                ViewBag.CommissionModes = new SelectList(Enum.GetValues(typeof(CommissionMode)), university.CommissionMode);
                ViewBag.UniversityTypes = new SelectList(Enum.GetValues(typeof(UniversityType)), university.Type);
                return View(university);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var university = await _context.Universities
                    .Include(u => u.Representatives)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (university == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                university.IsActive = true;
                university.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                foreach (var rep in university.Representatives.Where(r => r.IsActive))
                {
                    var notification = new Notification
                    {
                        UserId = rep.UserId,
                        Title = "University Approved",
                        Message = $"The university '{university.NameEnglish}' has been approved by the platform administrator.",
                        Category = NotificationCategory.SystemAlert,
                        ActionUrl = $"/UniversityRep/Dashboard",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Notifications.Add(notification);

                    // Send real-time notification via SignalR
                    await _hubContext.Clients.Group($"User_{rep.UserId}")
                        .SendAsync("ReceiveNotification", notification.Title, notification.Message, notification.ActionUrl);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"University approved: {university.NameEnglish} (ID: {id}) by admin {User.Identity?.Name}");

                TempData["Success"] = $"University '{university.NameEnglish}' has been approved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving university ID: {id}");
                TempData["Error"] = "An error occurred while approving the university.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suspend(int id, string? reason)
        {
            try
            {
                var university = await _context.Universities
                    .Include(u => u.Representatives)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (university == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                university.IsActive = false;
                university.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var suspensionMessage = string.IsNullOrWhiteSpace(reason)
                    ? $"The university '{university.NameEnglish}' has been suspended by the platform administrator."
                    : $"The university '{university.NameEnglish}' has been suspended. Reason: {reason}";

                foreach (var rep in university.Representatives.Where(r => r.IsActive))
                {
                    var notification = new Notification
                    {
                        UserId = rep.UserId,
                        Title = "University Suspended",
                        Message = suspensionMessage,
                        Category = NotificationCategory.SystemAlert,
                        ActionUrl = $"/UniversityRep/Dashboard",
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Notifications.Add(notification);

                    await _hubContext.Clients.Group($"User_{rep.UserId}")
                        .SendAsync("ReceiveNotification", notification.Title, notification.Message, notification.ActionUrl);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"University suspended: {university.NameEnglish} (ID: {id}) by admin {User.Identity?.Name}. Reason: {reason ?? "Not specified"}");

                TempData["Success"] = $"University '{university.NameEnglish}' has been suspended successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error suspending university ID: {id}");
                TempData["Error"] = "An error occurred while suspending the university.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Representatives(int id)
        {
            try
            {
                var university = await _context.Universities
                    .Include(u => u.Representatives)
                        .ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (university == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                var existingRepUserIds = university.Representatives.Select(r => r.UserId).ToList();
                var availableUsers = await _userManager.GetUsersInRoleAsync(UserRoles.UniversityRep);
                var availableReps = availableUsers
                    .Where(u => !existingRepUserIds.Contains(u.Id))
                    .Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = $"{u.FullName} ({u.Email})"
                    })
                    .ToList();

                ViewBag.University = university;
                ViewBag.AvailableRepresentatives = availableReps;

                return View(university.Representatives.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading representatives for university ID: {id}");
                TempData["Error"] = "An error occurred while loading university representatives.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRepresentative(int id, string userId)
        {
            try
            {
                var university = await _context.Universities.FindAsync(id);
                if (university == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Representatives), new { id });
                }

                var existingRep = await _context.UniversityRepresentatives
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.UniversityId == id);

                if (existingRep != null)
                {
                    TempData["Error"] = "This user is already a representative of this university.";
                    return RedirectToAction(nameof(Representatives), new { id });
                }

                var isInRole = await _userManager.IsInRoleAsync(user, UserRoles.UniversityRep);
                if (!isInRole)
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.UniversityRep);
                }

                var representative = new UniversityRepresentative
                {
                    UserId = userId,
                    UniversityId = id,
                    IsActive = true,
                };

                _context.UniversityRepresentatives.Add(representative);
                await _context.SaveChangesAsync();

                // Send notification to the user
                var notification = new Notification
                {
                    UserId = userId,
                    Title = "University Representative Assignment",
                    Message = $"You have been assigned as a representative for '{university.NameEnglish}'.",
                    Category = NotificationCategory.SystemAlert,
                    ActionUrl = "/UniversityRep/Dashboard",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send real-time notification
                await _hubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", notification.Title, notification.Message, notification.ActionUrl);

                _logger.LogInformation($"Representative added: User {user.Email} to University {university.NameEnglish} (ID: {id}) by admin {User.Identity?.Name}");

                TempData["Success"] = $"{user.FullName} has been added as a representative for '{university.NameEnglish}'.";
                return RedirectToAction(nameof(Representatives), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding representative to university ID: {id}");
                TempData["Error"] = "An error occurred while adding the representative.";
                return RedirectToAction(nameof(Representatives), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRepresentative(int id, int repId)
        {
            try
            {
                var representative = await _context.UniversityRepresentatives
                    .Include(r => r.User)
                    .Include(r => r.University)
                    .FirstOrDefaultAsync(r => r.Id == repId && r.UniversityId == id);

                if (representative == null)
                {
                    TempData["Error"] = "Representative not found.";
                    return RedirectToAction(nameof(Representatives), new { id });
                }

                var userName = representative.User.FullName;
                var universityName = representative.University.NameEnglish;
                var userId = representative.UserId;

                _context.UniversityRepresentatives.Remove(representative);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    UserId = userId,
                    Title = "University Representative Assignment Removed",
                    Message = $"Your role as a representative for '{universityName}' has been removed by the platform administrator.",
                    Category = NotificationCategory.SystemAlert,
                    ActionUrl = "/Account/Profile",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", notification.Title, notification.Message, notification.ActionUrl);

                _logger.LogInformation($"Representative removed: User {userName} from University {universityName} (ID: {id}) by admin {User.Identity?.Name}");

                TempData["Success"] = $"{userName} has been removed as a representative for '{universityName}'.";
                return RedirectToAction(nameof(Representatives), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing representative ID: {repId} from university ID: {id}");
                TempData["Error"] = "An error occurred while removing the representative.";
                return RedirectToAction(nameof(Representatives), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var university = await _context.Universities
                    .Include(u => u.Representatives)
                        .ThenInclude(r => r.User)
                    .Include(u => u.UniversityPrograms)
                        .ThenInclude(up => up.Program)
                    .Include(u => u.BtecPrograms)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (university == null)
                {
                    TempData["Error"] = "University not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.TotalPrograms = university.UniversityPrograms.Count + university.BtecPrograms.Count;
                ViewBag.ActivePrograms = university.UniversityPrograms.Count(up => up.IsActive) + university.BtecPrograms.Count(bp => bp.IsActive);
                ViewBag.TotalRepresentatives = university.Representatives.Count;
                ViewBag.ActiveRepresentatives = university.Representatives.Count(r => r.IsActive);

                var applicationStats = await _context.StudentApplications
                    .Where(sa => sa.UniversityProgram!.UniversityId == id || sa.BtecProgram!.UniversityId == id)
                    .GroupBy(sa => sa.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync();

                ViewBag.ApplicationStats = applicationStats;

                return View(university);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading details for university ID: {id}");
                TempData["Error"] = "An error occurred while loading university details.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
