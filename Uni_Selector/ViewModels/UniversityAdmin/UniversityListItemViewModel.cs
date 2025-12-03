using Uni_Selector.Models.Enums;

namespace Uni_Selector.ViewModels.UniversityAdmin
{
    public class UniversityListItemViewModel
    {
        public int Id { get; set; }
        public string NameEnglish { get; set; } = string.Empty;
        public string NameArabic { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? LogoPath { get; set; }
        public bool IsActive { get; set; }
        public int RepresentativesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public CommissionMode CommissionMode { get; set; }
    }
}
