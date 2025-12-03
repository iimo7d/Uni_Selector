using Uni_Selector.Models.Enums;

namespace Uni_Selector.ViewModels.UniversityAdmin
{
    public class UniversityDetailsViewModel
    {
        public int Id { get; set; }
        public string NameEnglish { get; set; } = string.Empty;
        public string NameArabic { get; set; } = string.Empty;
        public UniversityType Type { get; set; }
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AcademicAccreditation { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? OfficialWebsite { get; set; }
        public string? LogoPath { get; set; }
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; }
        public CommissionMode CommissionMode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Statistics
        public int TotalPrograms { get; set; }
        public int ActivePrograms { get; set; }
        public int TotalRepresentatives { get; set; }
        public int ActiveRepresentatives { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }

        // Related data
        public List<RepresentativeItemViewModel> Representatives { get; set; } = new();
        public Dictionary<string, int> ApplicationStatistics { get; set; } = new();
    }
}
