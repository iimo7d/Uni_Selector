using System.ComponentModel.DataAnnotations;

namespace Uni_Selector.ViewModels.UniversityAdmin
{
    public class UniversityListViewModel
    {
        public List<UniversityListItemViewModel> Universities { get; set; } = new();

        [Display(Name = "Search")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Province")]
        public string? Province { get; set; }

        [Display(Name = "Status")]
        public bool? IsActive { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; }
        public int TotalUniversities { get; set; }
        public List<string> AvailableProvinces { get; set; } = new();
    }
}
