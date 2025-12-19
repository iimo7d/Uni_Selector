using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.Student
{
    public class PersonalInfoStepViewModel
    {
        [Required(ErrorMessage = "الرقم الوطني مطلوب")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "الرقم الوطني يجب أن يكون 10 أرقام")]
        [Display(Name = "الرقم الوطني")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الجلوس مطلوب")]
        [Display(Name = "رقم الجلوس")]
        public string SeatNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "الجنس مطلوب")]
        [Display(Name = "الجنس")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "الجنسية مطلوبة")]
        [Display(Name = "الجنسية")]
        public string Nationality { get; set; } = "Jordanian";

        [Required(ErrorMessage = "المحافظة مطلوبة")]
        [Display(Name = "المحافظة")]
        public string Province { get; set; } = string.Empty;

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [Display(Name = "المدينة")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "المنطقة")]
        public string? Area { get; set; }

        // Dropdown Lists
        public List<SelectListItem> Genders { get; set; } = new();
        public List<SelectListItem> Provinces { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
    }
}
