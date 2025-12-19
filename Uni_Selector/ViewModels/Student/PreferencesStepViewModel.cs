using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.Student
{
    public class PreferencesStepViewModel
    {
        [Required(ErrorMessage = "ميزانية التسجيل مطلوبة")]
        [Range(0, 100000, ErrorMessage = "الميزانية يجب أن تكون بين 0 و 100,000")]
        [Display(Name = "ميزانية التسجيل (دينار)")]
        public decimal RegistrationBudget { get; set; }

        [Display(Name = "التخصصات المرغوبة")]
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

        [Display(Name = "هل لديك صلة قرابة بجامعة؟")]
        public bool HasFamilyConnection { get; set; }

        [Display(Name = "الجامعة المرتبطة")]
        public int? FamilyConnectionUniversityId { get; set; }

        // Dropdown Lists
        public List<SelectListItem> Languages { get; set; } = new();
        public List<SelectListItem> Universities { get; set; } = new();
        public List<SelectListItem> PreferredCities { get; set; } = new();
    }
}
