using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.Student
{
    public class AcademicInfoStepViewModel
    {
        [Required(ErrorMessage = "المعدل مطلوب")]
        [Range(0, 100, ErrorMessage = "المعدل يجب أن يكون بين 0 و 100")]
        [Display(Name = "المعدل")]
        public double GPA { get; set; }

        [Required(ErrorMessage = "المسار التعليمي مطلوب")]
        [Display(Name = "المسار التعليمي")]
        public int Path { get; set; }

        [Display(Name = "الفرع الأكاديمي")]
        public int? AcademicTrack { get; set; }

        [Display(Name = "الفرع المهني")]
        public int? VocationalBranch { get; set; }

        [Display(Name = "هل أكملت BTEC Level 2؟")]
        public bool BtecLevel2Completed { get; set; }

        [Display(Name = "هل أكملت BTEC Level 3؟")]
        public bool BtecLevel3Completed { get; set; }

        // Dropdown Lists
        public List<SelectListItem> Paths { get; set; } = new();
        public List<SelectListItem> AcademicTracks { get; set; } = new();
        public List<SelectListItem> VocationalBranches { get; set; } = new();
    }
}
