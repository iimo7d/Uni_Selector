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
using Uni_Selector.ViewModels.Student;

namespace Uni_Selector.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StudentController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = environment;
        }

        #region Dashboard

        // GET: /Student/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.FamilyConnectionUniversity)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "لم يتم العثور على بيانات الطالب";
                return RedirectToAction("CompleteProfile");
            }

            // Check if profile is complete
            bool isProfileComplete = !string.IsNullOrEmpty(student.NationalId) &&
                                    !string.IsNullOrEmpty(student.GuardianName);

            if (!isProfileComplete)
            {
                TempData["Warning"] = "الرجاء إكمال بيانات ملفك الشخصي للحصول على التوصيات";
            }

            var viewModel = new StudentDashboardViewModel
            {
                StudentId = student.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                GPA = student.GPA,
                Province = student.Province,
                City = student.City,
                PathDisplay = GetPathDisplay(student.Path),
                TrackDisplay = GetTrackDisplay(student.Path, student.AcademicTrack, student.VocationalBranch),
                RegistrationBudget = student.RegistrationBudget,
                MemberSince = user.CreatedAt,
                IsProfileComplete = isProfileComplete,

                // Statistics
                TotalRecommendations = await _context.Recommendations
                    .CountAsync(r => r.StudentId == student.Id),
                ViewedRecommendations = await _context.Recommendations
                    .CountAsync(r => r.StudentId == student.Id && r.IsViewed),
                TotalApplications = await _context.StudentApplications
                    .CountAsync(a => a.StudentId == student.Id),
                PendingApplications = await _context.StudentApplications
                    .CountAsync(a => a.StudentId == student.Id && a.Status == ApplicationStatus.Pending),
                ApprovedApplications = await _context.StudentApplications
                    .CountAsync(a => a.StudentId == student.Id && a.Status == ApplicationStatus.Approved),
                ActiveDiscounts = await _context.DiscountGrants
                    .CountAsync(d => d.Application.StudentId == student.Id && d.Status == DiscountStatus.Issued),

                // Recent Recommendations
                RecentRecommendations = await _context.Recommendations
                    .Where(r => r.StudentId == student.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new RecentRecommendationViewModel
                    {
                        Id = r.Id,
                        ProgramName = r.UniversityProgram != null
                            ? r.UniversityProgram.Program.NameArabic
                            : r.BtecProgram!.NameArabic,
                        UniversityName = r.UniversityProgram != null
                            ? r.UniversityProgram.University.NameArabic
                            : r.BtecProgram!.University.NameArabic,
                        Score = r.Score,
                        EstimatedCost = r.EstimatedTotalCost,
                        CreatedAt = r.CreatedAt,
                        IsViewed = r.IsViewed
                    })
                    .ToListAsync(),

                // Recent Applications
                RecentApplications = await _context.StudentApplications
                    .Where(a => a.StudentId == student.Id)
                    .OrderByDescending(a => a.ApplicationDate)
                    .Take(5)
                    .Select(a => new RecentApplicationViewModel
                    {
                        Id = a.Id,
                        ApplicationNumber = a.ApplicationNumber ?? $"APP-{a.Id:D6}",
                        ProgramName = a.UniversityProgram != null
                            ? a.UniversityProgram.Program.NameArabic
                            : a.BtecProgram!.NameArabic,
                        UniversityName = a.UniversityProgram != null
                            ? a.UniversityProgram.University.NameArabic
                            : a.BtecProgram!.University.NameArabic,
                        Status = GetStatusDisplay(a.Status),
                        StatusBadgeClass = GetStatusBadgeClass(a.Status),
                        ApplicationDate = a.ApplicationDate
                    })
                    .ToListAsync(),

                // Recent Notifications
                RecentNotifications = await _context.Notifications
                    .Where(n => n.UserId == user.Id)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .Select(n => new NotificationViewModel
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Message = n.Message,
                        Category = n.Category.ToString(),
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt,
                        ActionUrl = n.ActionUrl
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        #endregion


        #region Student Details

        // GET: /Student/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .Include(s => s.User)
                .Include(s => s.FamilyConnectionUniversity)
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "لم يتم العثور على بيانات الطالب";
                return RedirectToAction("Dashboard");
            }

            var viewModel = new StudentDetailsViewModel
            {
                Id = student.Id,
                UserId = student.UserId,
                FullName = student.User.FullName,
                Email = student.User.Email ?? string.Empty,
                PhoneNumber = student.User.PhoneNumber,
                MemberSince = student.User.CreatedAt,
                LastLogin = student.User.LastLoginAt,

                // Personal Information
                NationalId = student.NationalId,
                SeatNumber = student.SeatNumber,
                Gender = student.Gender == Gender.Male ? "ذكر" : "أنثى",
                DateOfBirth = student.DateOfBirth,
                Nationality = student.Nationality,

                // Location
                Province = student.Province,
                City = student.City,
                Area = student.Area,
                Latitude = student.Latitude,
                Longitude = student.Longitude,

                // Academic Information
                GPA = student.GPA,
                Path = GetPathDisplay(student.Path),
                AcademicTrack = student.AcademicTrack.HasValue ? GetAcademicTrackDisplay(student.AcademicTrack.Value) : null,
                VocationalBranch = student.VocationalBranch.HasValue ? GetVocationalBranchDisplay(student.VocationalBranch.Value) : null,
                BtecLevel2Completed = student.BtecLevel2Completed,
                BtecLevel3Completed = student.BtecLevel3Completed,
                BtecCertificateUrl = student.BtecCertificateUrl,

                // Preferences
                RegistrationBudget = student.RegistrationBudget,
                DesiredMajors = student.DesiredMajors,
                PreferredCity = student.PreferredCity,
                MaxDistanceKm = student.MaxDistanceKm,
                PreferredLanguage = GetLanguageDisplay(student.PreferredLanguage),

                // Family Connection
                HasFamilyConnection = student.HasFamilyConnection,
                FamilyConnectionUniversityName = student.FamilyConnectionUniversity?.NameArabic,

                // Guardian Information
                GuardianName = student.GuardianName,
                GuardianPhone = student.GuardianPhone,
                GuardianRelation = student.GuardianRelation,

                // Special Circumstances
                HasDisability = student.HasDisability,
                DisabilityType = student.DisabilityType,
                IsOrphan = student.IsOrphan,
                IsEmployeeChild = student.IsEmployeeChild,

                // Statistics
                TotalRecommendations = await _context.Recommendations.CountAsync(r => r.StudentId == student.Id),
                TotalApplications = await _context.StudentApplications.CountAsync(a => a.StudentId == student.Id)
            };

            return View(viewModel);
        }

        #endregion

        #region Edit Profile

        // GET: /Student/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "لم يتم العثور على بيانات الطالب";
                return RedirectToAction("CompleteProfile");
            }

            var viewModel = new EditProfileViewModel
            {
                StudentId = student.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Province = student.Province,
                City = student.City,
                Area = student.Area,
                RegistrationBudget = student.RegistrationBudget,
                DesiredMajors = student.DesiredMajors,
                PreferredCity = student.PreferredCity,
                MaxDistanceKm = student.MaxDistanceKm,
                PreferredLanguage = (int)student.PreferredLanguage,
                HasFamilyConnection = student.HasFamilyConnection,
                FamilyConnectionUniversityId = student.FamilyConnectionUniversityId,
                GuardianName = student.GuardianName,
                GuardianPhone = student.GuardianPhone,
                GuardianRelation = student.GuardianRelation
            };

            await PopulateEditProfileDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: /Student/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateEditProfileDropdowns(model);
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "لم يتم العثور على بيانات الطالب";
                return RedirectToAction("CompleteProfile");
            }

            try
            {
                // Update User
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                await _userManager.UpdateAsync(user);

                // Update Student
                student.Province = model.Province;
                student.City = model.City;
                student.Area = model.Area;
                student.RegistrationBudget = model.RegistrationBudget;
                student.DesiredMajors = model.DesiredMajors;
                student.PreferredCity = model.PreferredCity;
                student.MaxDistanceKm = model.MaxDistanceKm;
                student.PreferredLanguage = (LanguageCode)model.PreferredLanguage;
                student.HasFamilyConnection = model.HasFamilyConnection;
                student.FamilyConnectionUniversityId = model.HasFamilyConnection ? model.FamilyConnectionUniversityId : null;
                student.GuardianName = model.GuardianName;
                student.GuardianPhone = model.GuardianPhone;
                student.GuardianRelation = model.GuardianRelation;
                student.UpdatedAt = DateTime.UtcNow;

                _context.Students.Update(student);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم تحديث بيانات الملف الشخصي بنجاح";
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء تحديث البيانات";
                await PopulateEditProfileDropdowns(model);
                return View(model);
            }
        }

        #endregion

        #region Complete Profile Wizard

        // GET: /Student/CompleteProfile
        public async Task<IActionResult> CompleteProfile(int step = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            // Create student if not exists
            if (student == null)
            {
                student = new Student
                {
                    UserId = user.Id,
                    Province = string.Empty,
                    City = string.Empty,
                    GPA = 0,
                    Path = PathType.Academic,
                    RegistrationBudget = 0,
                    NationalId = string.Empty,
                    SeatNumber = string.Empty,
                    Gender = Gender.Male,
                    DateOfBirth = DateTime.Now.AddYears(-18),
                    GuardianName = string.Empty,
                    GuardianPhone = string.Empty,
                    GuardianRelation = string.Empty
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
            }

            var viewModel = new CompleteProfileViewModel
            {
                CurrentStep = step,
                StudentId = student.Id
            };

            await PopulateCompleteProfileDropdowns(viewModel, step);
            PopulateViewModelFromStudent(viewModel, student);

            return View(viewModel);
        }

        // POST: /Student/CompleteProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(CompleteProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "لم يتم العثور على بيانات الطالب";
                return RedirectToAction("CompleteProfile", new { step = 1 });
            }

            // Validate current step
            bool isValid = ValidateStep(model, model.CurrentStep);
            if (!isValid)
            {
                await PopulateCompleteProfileDropdowns(model, model.CurrentStep);
                return View(model);
            }

            try
            {
                // Save current step data
                SaveStepData(student, model, model.CurrentStep);
                student.UpdatedAt = DateTime.UtcNow;

                _context.Students.Update(student);
                await _context.SaveChangesAsync();

                // Move to next step or complete
                if (model.CurrentStep < model.TotalSteps)
                {
                    return RedirectToAction("CompleteProfile", new { step = model.CurrentStep + 1 });
                }
                else
                {
                    TempData["Success"] = "تم إكمال بيانات الملف الشخصي بنجاح! يمكنك الآن الحصول على توصيات الجامعات.";
                    return RedirectToAction("Dashboard");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "حدث خطأ أثناء حفظ البيانات";
                await PopulateCompleteProfileDropdowns(model, model.CurrentStep);
                return View(model);
            }
        }

        #endregion

        #region BTEC Certificate Upload

        // POST: /Student/UploadBTECCertificate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadBTECCertificate(IFormFile certificateFile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "غير مصرح" });

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
                return Json(new { success = false, message = "لم يتم العثور على بيانات الطالب" });

            if (certificateFile == null || certificateFile.Length == 0)
                return Json(new { success = false, message = "الرجاء اختيار ملف" });

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(certificateFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return Json(new { success = false, message = "نوع الملف غير مدعوم. يرجى رفع PDF أو صورة" });

            // Validate file size (max 5MB)
            if (certificateFile.Length > 5 * 1024 * 1024)
                return Json(new { success = false, message = "حجم الملف يجب أن يكون أقل من 5 ميجابايت" });

            try
            {
                // Create uploads directory if not exists
                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "btec-certificates");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileName = $"{student.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Delete old file if exists
                if (!string.IsNullOrEmpty(student.BtecCertificateUrl))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, student.BtecCertificateUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Save new file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await certificateFile.CopyToAsync(stream);
                }

                // Update student record
                student.BtecCertificateUrl = $"/uploads/btec-certificates/{fileName}";
                student.UpdatedAt = DateTime.UtcNow;

                _context.Students.Update(student);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "تم رفع الشهادة بنجاح",
                    fileUrl = student.BtecCertificateUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء رفع الملف" });
            }
        }

        #endregion


        #region Helper Methods

        private async Task PopulateEditProfileDropdowns(EditProfileViewModel model)
        {
            model.Provinces = GetJordanProvinces();
            model.Cities = GetCitiesByProvince(model.Province);
            model.Languages = GetLanguages();
            model.Universities = await GetUniversities();
        }

        private async Task PopulateCompleteProfileDropdowns(CompleteProfileViewModel model, int step)
        {
            switch (step)
            {
                case 1:
                    model.PersonalInfo.Genders = GetGenders();
                    model.PersonalInfo.Provinces = GetJordanProvinces();
                    model.PersonalInfo.Cities = GetCitiesByProvince(model.PersonalInfo.Province);
                    break;
                case 2:
                    model.AcademicInfo.Paths = GetPaths();
                    model.AcademicInfo.AcademicTracks = GetAcademicTracks();
                    model.AcademicInfo.VocationalBranches = GetVocationalBranches();
                    break;
                case 3:
                    model.Preferences.Languages = GetLanguages();
                    model.Preferences.Universities = await GetUniversities();
                    model.Preferences.PreferredCities = GetJordanCities();
                    break;
            }
        }

        private void PopulateViewModelFromStudent(CompleteProfileViewModel model, Student student)
        {
            // Personal Info
            model.PersonalInfo.NationalId = student.NationalId;
            model.PersonalInfo.SeatNumber = student.SeatNumber;
            model.PersonalInfo.Gender = (int)student.Gender;
            model.PersonalInfo.DateOfBirth = student.DateOfBirth;
            model.PersonalInfo.Nationality = student.Nationality;
            model.PersonalInfo.Province = student.Province;
            model.PersonalInfo.City = student.City;
            model.PersonalInfo.Area = student.Area;

            // Academic Info
            model.AcademicInfo.GPA = student.GPA;
            model.AcademicInfo.Path = (int)student.Path;
            model.AcademicInfo.AcademicTrack = student.AcademicTrack.HasValue ? (int)student.AcademicTrack : null;
            model.AcademicInfo.VocationalBranch = student.VocationalBranch.HasValue ? (int)student.VocationalBranch : null;
            model.AcademicInfo.BtecLevel2Completed = student.BtecLevel2Completed;
            model.AcademicInfo.BtecLevel3Completed = student.BtecLevel3Completed;

            // Preferences
            model.Preferences.RegistrationBudget = student.RegistrationBudget;
            model.Preferences.DesiredMajors = student.DesiredMajors;
            model.Preferences.PreferredCity = student.PreferredCity;
            model.Preferences.MaxDistanceKm = student.MaxDistanceKm;
            model.Preferences.PreferredLanguage = (int)student.PreferredLanguage;
            model.Preferences.HasFamilyConnection = student.HasFamilyConnection;
            model.Preferences.FamilyConnectionUniversityId = student.FamilyConnectionUniversityId;

            // Guardian
            model.Guardian.GuardianName = student.GuardianName;
            model.Guardian.GuardianPhone = student.GuardianPhone;
            model.Guardian.GuardianRelation = student.GuardianRelation;
            model.Guardian.HasDisability = student.HasDisability;
            model.Guardian.DisabilityType = student.DisabilityType;
            model.Guardian.IsOrphan = student.IsOrphan;
            model.Guardian.IsEmployeeChild = student.IsEmployeeChild;
        }

        private bool ValidateStep(CompleteProfileViewModel model, int step)
        {
            switch (step)
            {
                case 1:
                    return !string.IsNullOrEmpty(model.PersonalInfo.NationalId) &&
                           !string.IsNullOrEmpty(model.PersonalInfo.SeatNumber) &&
                           !string.IsNullOrEmpty(model.PersonalInfo.Province) &&
                           !string.IsNullOrEmpty(model.PersonalInfo.City);
                case 2:
                    return model.AcademicInfo.GPA >= 0 && model.AcademicInfo.GPA <= 100;
                case 3:
                    return model.Preferences.RegistrationBudget >= 0;
                case 4:
                    return !string.IsNullOrEmpty(model.Guardian.GuardianName) &&
                           !string.IsNullOrEmpty(model.Guardian.GuardianPhone);
                default:
                    return true;
            }
        }

        private void SaveStepData(Student student, CompleteProfileViewModel model, int step)
        {
            switch (step)
            {
                case 1:
                    student.NationalId = model.PersonalInfo.NationalId;
                    student.SeatNumber = model.PersonalInfo.SeatNumber;
                    student.Gender = (Gender)model.PersonalInfo.Gender;
                    student.DateOfBirth = model.PersonalInfo.DateOfBirth;
                    student.Nationality = model.PersonalInfo.Nationality;
                    student.Province = model.PersonalInfo.Province;
                    student.City = model.PersonalInfo.City;
                    student.Area = model.PersonalInfo.Area;
                    break;
                case 2:
                    student.GPA = model.AcademicInfo.GPA;
                    student.Path = (PathType)model.AcademicInfo.Path;
                    student.AcademicTrack = model.AcademicInfo.AcademicTrack.HasValue
                        ? (AcademicTrack)model.AcademicInfo.AcademicTrack : null;
                    student.VocationalBranch = model.AcademicInfo.VocationalBranch.HasValue
                        ? (VocationalBranch)model.AcademicInfo.VocationalBranch : null;
                    student.BtecLevel2Completed = model.AcademicInfo.BtecLevel2Completed;
                    student.BtecLevel3Completed = model.AcademicInfo.BtecLevel3Completed;
                    break;
                case 3:
                    student.RegistrationBudget = model.Preferences.RegistrationBudget;
                    student.DesiredMajors = model.Preferences.DesiredMajors;
                    student.PreferredCity = model.Preferences.PreferredCity;
                    student.MaxDistanceKm = model.Preferences.MaxDistanceKm;
                    student.PreferredLanguage = (LanguageCode)model.Preferences.PreferredLanguage;
                    student.HasFamilyConnection = model.Preferences.HasFamilyConnection;
                    student.FamilyConnectionUniversityId = model.Preferences.HasFamilyConnection
                        ? model.Preferences.FamilyConnectionUniversityId : null;
                    break;
                case 4:
                    student.GuardianName = model.Guardian.GuardianName;
                    student.GuardianPhone = model.Guardian.GuardianPhone;
                    student.GuardianRelation = model.Guardian.GuardianRelation;
                    student.HasDisability = model.Guardian.HasDisability;
                    student.DisabilityType = model.Guardian.HasDisability ? model.Guardian.DisabilityType : null;
                    student.IsOrphan = model.Guardian.IsOrphan;
                    student.IsEmployeeChild = model.Guardian.IsEmployeeChild;
                    break;
            }
        }

        #region Static Data Helpers

        private List<SelectListItem> GetJordanProvinces()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر المحافظة --" },
                new() { Value = "عمان", Text = "عمان" },
                new() { Value = "إربد", Text = "إربد" },
                new() { Value = "الزرقاء", Text = "الزرقاء" },
                new() { Value = "المفرق", Text = "المفرق" },
                new() { Value = "عجلون", Text = "عجلون" },
                new() { Value = "جرش", Text = "جرش" },
                new() { Value = "البلقاء", Text = "البلقاء" },
                new() { Value = "مادبا", Text = "مادبا" },
                new() { Value = "الكرك", Text = "الكرك" },
                new() { Value = "الطفيلة", Text = "الطفيلة" },
                new() { Value = "معان", Text = "معان" },
                new() { Value = "العقبة", Text = "العقبة" }
            };
        }

        private List<SelectListItem> GetJordanCities()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر المدينة --" },
                new() { Value = "عمان", Text = "عمان" },
                new() { Value = "إربد", Text = "إربد" },
                new() { Value = "الزرقاء", Text = "الزرقاء" },
                new() { Value = "العقبة", Text = "العقبة" },
                new() { Value = "السلط", Text = "السلط" },
                new() { Value = "المفرق", Text = "المفرق" },
                new() { Value = "الكرك", Text = "الكرك" },
                new() { Value = "مادبا", Text = "مادبا" },
                new() { Value = "جرش", Text = "جرش" },
                new() { Value = "عجلون", Text = "عجلون" },
                new() { Value = "معان", Text = "معان" },
                new() { Value = "الطفيلة", Text = "الطفيلة" }
            };
        }

        private List<SelectListItem> GetCitiesByProvince(string province)
        {
            // Return cities based on province
            return GetJordanCities();
        }

        private List<SelectListItem> GetGenders()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر الجنس --" },
                new() { Value = "1", Text = "ذكر" },
                new() { Value = "2", Text = "أنثى" }
            };
        }

        private List<SelectListItem> GetPaths()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر المسار --" },
                new() { Value = "1", Text = "أكاديمي" },
                new() { Value = "2", Text = "مهني" },
                new() { Value = "3", Text = "BTEC" }
            };
        }

        private List<SelectListItem> GetAcademicTracks()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر الفرع --" },
                new() { Value = "1", Text = "علمي" },
                new() { Value = "2", Text = "أدبي" },
                new() { Value = "3", Text = "شرعي" }
            };
        }

        private List<SelectListItem> GetVocationalBranches()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر الفرع --" },
                new() { Value = "1", Text = "صناعي" },
                new() { Value = "2", Text = "زراعي" },
                new() { Value = "3", Text = "فندقي وسياحي" },
                new() { Value = "4", Text = "اقتصاد منزلي" }
            };
        }

        private List<SelectListItem> GetLanguages()
        {
            return new List<SelectListItem>
            {
                new() { Value = "", Text = "-- اختر اللغة --" },
                new() { Value = "1", Text = "الإنجليزية" },
                new() { Value = "2", Text = "العربية" },
                new() { Value = "3", Text = "كلاهما" }
            };
        }

        private async Task<List<SelectListItem>> GetUniversities()
        {
            var universities = await _context.Universities
                .Where(u => u.IsActive)
                .OrderBy(u => u.NameArabic)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.NameArabic
                })
                .ToListAsync();

            universities.Insert(0, new SelectListItem { Value = "", Text = "-- اختر الجامعة --" });
            return universities;
        }

        private string GetPathDisplay(PathType path)
        {
            return path switch
            {
                PathType.Academic => "أكاديمي",
                PathType.Vocational => "مهني",
                PathType.BTEC => "BTEC",
                _ => "غير محدد"
            };
        }

        private string GetAcademicTrackDisplay(AcademicTrack track)
        {
            return track switch
            {
                AcademicTrack.Scientific => "علمي",
                AcademicTrack.Literary => "أدبي",
                AcademicTrack.Sharia => "شرعي",
                _ => "غير محدد"
            };
        }

        private string GetVocationalBranchDisplay(VocationalBranch branch)
        {
            return branch switch
            {
                VocationalBranch.Industrial => "صناعي",
                VocationalBranch.Agricultural => "زراعي",
                VocationalBranch.HotelTourism => "فندقي وسياحي",
                VocationalBranch.HomeEconomics => "اقتصاد منزلي",
                _ => "غير محدد"
            };
        }

        private string? GetTrackDisplay(PathType path, AcademicTrack? academicTrack, VocationalBranch? vocationalBranch)
        {
            return path switch
            {
                PathType.Academic when academicTrack.HasValue => GetAcademicTrackDisplay(academicTrack.Value),
                PathType.Vocational when vocationalBranch.HasValue => GetVocationalBranchDisplay(vocationalBranch.Value),
                _ => null
            };
        }

        private string GetLanguageDisplay(LanguageCode language)
        {
            return language switch
            {
                LanguageCode.English => "الإنجليزية",
                LanguageCode.Arabic => "العربية",
                LanguageCode.Both => "كلاهما",
                _ => "غير محدد"
            };
        }

        private static string GetStatusDisplay(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "قيد الانتظار",
                ApplicationStatus.UnderReview => "قيد المراجعة",
                ApplicationStatus.Approved => "مقبول",
                ApplicationStatus.Rejected => "مرفوض",
                ApplicationStatus.Enrolled => "مسجل",
                ApplicationStatus.Cancelled => "ملغي",
                _ => "غير محدد"
            };
        }

        private static string GetStatusBadgeClass(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "bg-warning",
                ApplicationStatus.UnderReview => "bg-info",
                ApplicationStatus.Approved => "bg-success",
                ApplicationStatus.Rejected => "bg-danger",
                ApplicationStatus.Enrolled => "bg-primary",
                ApplicationStatus.Cancelled => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        #endregion

        #endregion

    }
}
