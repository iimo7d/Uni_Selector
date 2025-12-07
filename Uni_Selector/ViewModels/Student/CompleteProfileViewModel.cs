using System.ComponentModel.DataAnnotations;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.ViewModels.Student
{
    public class CompleteProfileViewModel
    {
        // Wizard tracking
        public int CurrentStep { get; set; } = 1;
        public int TotalSteps { get; set; } = 4;

        // Step 1: Personal Information + Location
        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required, Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Province/Governorate")]
        public string Province { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(200)]
        [Display(Name = "Area/District (Optional)")]
        public string? Area { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        // Step 2: Academic Information
        [Required, Range(0, 100)]
        [Display(Name = "GPA (High School/Tawjihi %)")]
        public double GPA { get; set; }

        [Required]
        [Display(Name = "Educational Path")]
        public PathType Path { get; set; }

        [Display(Name = "Academic Track")]
        public AcademicTrack? AcademicTrack { get; set; }

        [Display(Name = "Vocational Branch")]
        public VocationalBranch? VocationalBranch { get; set; }

        [Display(Name = "BTEC Level 2 Completed")]
        public bool BtecLevel2Completed { get; set; }

        [Display(Name = "BTEC Level 3 Completed")]
        public bool BtecLevel3Completed { get; set; }

        // Step 3: Financial & Preferences
        [Required, Range(1, 999999)]
        [Display(Name = "Registration Budget (JOD)")]
        public decimal RegistrationBudget { get; set; }

        [StringLength(500)]
        [Display(Name = "Desired Majors (separate with commas)")]
        public string? DesiredMajors { get; set; }

        [StringLength(100)]
        [Display(Name = "Preferred City (Optional)")]
        public string? PreferredCity { get; set; }

        [Range(0, 500)]
        [Display(Name = "Maximum Distance from Home (km)")]
        public int MaxDistanceKm { get; set; } = 50;

        [Display(Name = "Preferred Teaching Language")]
        public LanguageCode PreferredLanguage { get; set; } = LanguageCode.English;

        // Step 4: Additional Information
        [Display(Name = "I have a sibling/family member at a university")]
        public bool HasFamilyConnection { get; set; }

        [Display(Name = "Select University")]
        public int? FamilyConnectionUniversityId { get; set; }
    }
}
