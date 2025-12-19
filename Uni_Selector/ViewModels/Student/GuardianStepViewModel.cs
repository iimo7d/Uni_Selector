using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.Student
{
    public class GuardianStepViewModel
    {
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

        [Display(Name = "هل لديك إعاقة؟")]
        public bool HasDisability { get; set; }

        [Display(Name = "نوع الإعاقة")]
        public string? DisabilityType { get; set; }

        [Display(Name = "يتيم")]
        public bool IsOrphan { get; set; }

        [Display(Name = "ابن موظف")]
        public bool IsEmployeeChild { get; set; }
    }
}
