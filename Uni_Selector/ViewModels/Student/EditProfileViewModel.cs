using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.Student
{
    public class EditProfileViewModel
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        // Location
        [Required(ErrorMessage = "المحافظة مطلوبة")]
        [Display(Name = "المحافظة")]
        public string Province { get; set; } = string.Empty;

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [Display(Name = "المدينة")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "المنطقة")]
        public string? Area { get; set; }

        // Preferences
        [Required(ErrorMessage = "ميزانية التسجيل مطلوبة")]
        [Range(0, 100000, ErrorMessage = "الميزانية يجب أن تكون بين 0 و 100,000")]
        [Display(Name = "ميزانية التسجيل (دينار)")]
        public decimal RegistrationBudget { get; set; }

        [Display(Name = "التخصصات المرغوبة")]
        [StringLength(500, ErrorMessage = "التخصصات لا يمكن أن تتجاوز 500 حرف")]
        public string? DesiredMajors { get; set; }

        [Display(Name = "المدينة المفضلة")]
        public string? PreferredCity { get; set; }

        [Required(ErrorMessage = "المسافة القصوى مطلوبة")]
        [Range(1, 500, ErrorMessage = "المسافة يجب أن تكون بين 1 و 500 كم")]
        [Display(Name = "المسافة القصوى (كم)")]
        public int MaxDistanceKm { get; set; } = 50;

        [Required(ErrorMessage = "لغة الدراسة المفضلة مطلوبة")]
        [Display(Name = "لغة الدراسة المفضلة")]
        public int PreferredLanguage { get; set; }

        // Family Connection
        [Display(Name = "هل لديك صلة قرابة بجامعة؟")]
        public bool HasFamilyConnection { get; set; }

        [Display(Name = "الجامعة المرتبطة")]
        public int? FamilyConnectionUniversityId { get; set; }

        // Guardian Information
        [Required(ErrorMessage = "اسم ولي الأمر مطلوب")]
        [Display(Name = "اسم ولي الأمر")]
        public string GuardianName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم هاتف ولي الأمر مطلوب")]
        [Phone(ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        [Display(Name = "رقم هاتف ولي الأمر")]
        public string GuardianPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "صلة القرابة مطلوبة")]
        [Display(Name = "صلة القرابة")]
        public string GuardianRelation { get; set; } = string.Empty;

        // Dropdown Lists
        public List<SelectListItem> Provinces { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
        public List<SelectListItem> Languages { get; set; } = new();
        public List<SelectListItem> Universities { get; set; } = new();
    }
}
