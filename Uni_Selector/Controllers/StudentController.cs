using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Uni_Selector.Data;
using Uni_Selector.Models;
using Uni_Selector.Models.Enums;
using Uni_Selector.ViewModels.Students;

namespace Uni_Selector.Controllers
{
    [Authorize(Roles = UserRoles.Student)]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
      AppDbContext context,
      UserManager<ApplicationUser> userManager,
      IWebHostEnvironment webHostEnvironment,
      ILogger<StudentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        // GET: /Student/CompleteProfile
        [HttpGet]
        public async Task<IActionResult> CompleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Check if student record already exists
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (existingStudent != null && existingStudent.ProfileCompleted == true)
            {
                TempData["Info"] = "Your profile is already completed. You can edit it instead.";
                return RedirectToAction(nameof(Edit));
            }

            var model = new CompleteProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            await PopulateViewBagData();
            return View(model);
        }

        // POST: /Student/CompleteProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagData();
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    throw new UnauthorizedAccessException();

                var existingStudent = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

                if (existingStudent != null && existingStudent.ProfileCompleted == true)
                {
                    TempData["Error"] = "Your profile is already completed. You can edit it instead.";
                    return RedirectToAction(nameof(Edit));
                }

                string? btecCertificateUrl = null;
                if (model.BtecCertificateFile != null)
                {
                    btecCertificateUrl = await SaveBtecCertificate(model.BtecCertificateFile, user.Id);
                }

                var student = existingStudent ?? new Student();

                student.UserId = user.Id;
                student.NationalId = model.NationalId;
                student.SeatNumber = model.SeatNumber;
                student.Gender = model.Gender;
                student.DateOfBirth = model.DateOfBirth;
                student.Nationality = model.Nationality ?? "Jordanian";

                student.GuardianName = model.GuardianName;
                student.GuardianPhone = model.GuardianPhone;
                student.GuardianRelation = model.GuardianRelation;

                student.Province = model.Province;
                student.City = model.City;
                student.Area = model.Area;

                student.GPA = model.GPA;
                student.Path = model.Path;
                student.AcademicTrack = model.AcademicTrack;
                student.VocationalBranch = model.VocationalBranch;
                student.BtecLevel2Completed = model.BtecLevel2Completed;
                student.BtecLevel3Completed = model.BtecLevel3Completed;
                student.BtecCertificateUrl = btecCertificateUrl;

                student.RegistrationBudget = model.RegistrationBudget;
                student.DesiredMajors = model.DesiredMajors;
                student.PreferredCity = model.PreferredCity;
                student.MaxDistanceKm = model.MaxDistanceKm;
                student.PreferredLanguage = model.PreferredLanguage;

                student.HasDisability = model.HasDisability;
                student.DisabilityType = model.DisabilityType;
                student.IsOrphan = model.IsOrphan;
                student.IsEmployeeChild = model.IsEmployeeChild;
                student.HasFamilyConnection = model.HasFamilyConnection;
                student.FamilyConnectionUniversityId = model.FamilyConnectionUniversityId;

                student.ProfileCompleted = true;
                student.IsActive = true;
                student.UpdatedAt = DateTime.UtcNow;

                if (existingStudent == null)
                {
                    student.CreatedAt = DateTime.UtcNow;
                    _context.Students.Add(student);
                }
                else
                {
                    _context.Students.Update(student);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Profile completed successfully!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex,
                    "Unauthorized access while completing profile. UserId: {UserId}",
                    User?.FindFirstValue(ClaimTypes.NameIdentifier));

                TempData["Error"] = "Your session has expired. Please login again.";
                return RedirectToAction("Login", "Account");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex,
                    "Database error while completing profile. UserId: {UserId}",
                    User?.FindFirstValue(ClaimTypes.NameIdentifier));

                TempData["Error"] = "A database error occurred. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while completing profile. UserId: {UserId}");

                TempData["Error"] = "Unexpected error occurred while saving your profile.";
            }


            await PopulateViewBagData();
            return View(model);
        }
        

        // GET: /Student/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null || student.ProfileCompleted != true)
            {
                TempData["Info"] = "Please complete your profile first.";
                return RedirectToAction(nameof(CompleteProfile));
            }

            var model = new EditProfileViewModel
            {
                // Personal Information
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                NationalId = student.NationalId,
                SeatNumber = student.SeatNumber,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                Nationality = student.Nationality,

                // Guardian Information
                GuardianName = student.GuardianName,
                GuardianPhone = student.GuardianPhone,
                GuardianRelation = student.GuardianRelation,

                // Location Information
                Province = student.Province,
                City = student.City,
                Area = student.Area,

                // Academic Information
                GPA = student.GPA,
                Path = student.Path,
                AcademicTrack = student.AcademicTrack,
                VocationalBranch = student.VocationalBranch,
                BtecLevel2Completed = student.BtecLevel2Completed,
                BtecLevel3Completed = student.BtecLevel3Completed,
                CurrentBtecCertificateUrl = student.BtecCertificateUrl,

                // Preferences
                RegistrationBudget = student.RegistrationBudget,
                DesiredMajors = student.DesiredMajors,
                PreferredCity = student.PreferredCity,
                MaxDistanceKm = student.MaxDistanceKm,
                PreferredLanguage = student.PreferredLanguage,

                // Special Circumstances
                HasDisability = student.HasDisability,
                DisabilityType = student.DisabilityType,
                IsOrphan = student.IsOrphan,
                IsEmployeeChild = student.IsEmployeeChild,
                HasFamilyConnection = student.HasFamilyConnection,
                FamilyConnectionUniversityId = student.FamilyConnectionUniversityId
            };

            await PopulateViewBagData();
            return View(model);
        }

        // POST: /Student/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewBagData();
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    throw new UnauthorizedAccessException();

                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == user.Id);

                if (student == null)
                {
                    TempData["Error"] = "Student profile not found.";
                    return RedirectToAction(nameof(CompleteProfile));
                }

                if (model.BtecCertificateFile != null)
                {
                    if (!string.IsNullOrEmpty(student.BtecCertificateUrl))
                    {
                        DeleteBtecCertificate(student.BtecCertificateUrl);
                    }

                    student.BtecCertificateUrl = await SaveBtecCertificate(model.BtecCertificateFile, user.Id);
                }

                student.NationalId = model.NationalId;
                student.SeatNumber = model.SeatNumber;
                student.Gender = model.Gender;
                student.DateOfBirth = model.DateOfBirth;
                student.Nationality = model.Nationality ?? "Jordanian";

                student.GuardianName = model.GuardianName;
                student.GuardianPhone = model.GuardianPhone;
                student.GuardianRelation = model.GuardianRelation;

                student.Province = model.Province;
                student.City = model.City;
                student.Area = model.Area;

                student.GPA = model.GPA;
                student.Path = model.Path;
                student.AcademicTrack = model.AcademicTrack;
                student.VocationalBranch = model.VocationalBranch;
                student.BtecLevel2Completed = model.BtecLevel2Completed;
                student.BtecLevel3Completed = model.BtecLevel3Completed;

                student.RegistrationBudget = model.RegistrationBudget;
                student.DesiredMajors = model.DesiredMajors;
                student.PreferredCity = model.PreferredCity;
                student.MaxDistanceKm = model.MaxDistanceKm;
                student.PreferredLanguage = model.PreferredLanguage;

                student.HasDisability = model.HasDisability;
                student.DisabilityType = model.DisabilityType;
                student.IsOrphan = model.IsOrphan;
                student.IsEmployeeChild = model.IsEmployeeChild;
                student.HasFamilyConnection = model.HasFamilyConnection;
                student.FamilyConnectionUniversityId = model.FamilyConnectionUniversityId;

                student.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex,
                    "Unauthorized access while editing profile. UserId: {UserId}",
                    User?.FindFirstValue(ClaimTypes.NameIdentifier));

                TempData["Error"] = "You are not authorized.";
                return RedirectToAction("Login", "Account");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex,
                    "Database error while editing profile. UserId: {UserId}",
                    User?.FindFirstValue(ClaimTypes.NameIdentifier));

                TempData["Error"] = "Database error occurred while updating profile.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while editing profile. UserId: {UserId}",
                    User?.FindFirstValue(ClaimTypes.NameIdentifier));

                TempData["Error"] = "Unexpected error occurred.";
            }


            await PopulateViewBagData();
            return View(model);
        }

        #region Helper Methods

        private async Task PopulateViewBagData()
        {
            // Get list of active universities for family connection dropdown
            var universities = await _context.Universities
                .Where(u => u.IsActive)
                .OrderBy(u => u.NameEnglish)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.NameEnglish
                })
                .ToListAsync();

            ViewBag.Universities = universities;

            // Jordanian Provinces
            ViewBag.Provinces = new List<string>
            {
                "Amman", "Irbid", "Zarqa", "Balqa", "Madaba",
                "Karak", "Tafilah", "Ma'an", "Aqaba", "Jerash",
                "Ajloun", "Mafraq"
            };

            // Guardian Relations
            ViewBag.GuardianRelations = new List<string>
            {
                "Father", "Mother", "Brother", "Sister",
                "Uncle", "Aunt", "Grandfather", "Grandmother", "Other"
            };
        }

        private async Task<string?> SaveBtecCertificate(IFormFile file, string userId)
        {
            try
            {
                var uploadsFolder = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    "uploads",
                    "btec-certificates");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                return $"/uploads/btec-certificates/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to save BTEC certificate. UserId: {UserId}, FileName: {FileName}",
                    userId,
                    file?.FileName);

                return null;
            }
        }


        private void DeleteBtecCertificate(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                    return;

                var filePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    fileUrl.TrimStart('/'));

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to delete BTEC certificate. FileUrl: {FileUrl}",
                    fileUrl);
            }
        }


        #endregion
    }
}
