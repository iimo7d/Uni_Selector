using System.ComponentModel.DataAnnotations;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.ViewModels.Student
{
    public class StudentProfileViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required, Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Province")]
        public string Province { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(200)]
        [Display(Name = "Area/District")]
        public string? Area { get; set; }

        [Display(Name = "Latitude")]
        public double? Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double? Longitude { get; set; }

        [Required, Range(0, 100)]
        [Display(Name = "GPA (%)")]
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

        [Display(Name = "BTEC Certificate")]
        public string? BtecCertificateUrl { get; set; }

        [Required, Range(0, 999999)]
        [Display(Name = "Registration Budget (JOD)")]
        public decimal RegistrationBudget { get; set; }

        [StringLength(500)]
        [Display(Name = "Desired Majors (comma-separated)")]
        public string? DesiredMajors { get; set; }

        [StringLength(100)]
        [Display(Name = "Preferred City")]
        public string? PreferredCity { get; set; }

        [Range(0, 500)]
        [Display(Name = "Maximum Distance (km)")]
        public int MaxDistanceKm { get; set; }

        [Display(Name = "Preferred Language")]
        public LanguageCode PreferredLanguage { get; set; }

        [Display(Name = "I have family at a university")]
        public bool HasFamilyConnection { get; set; }

        [Display(Name = "Family Connection University")]
        public int? FamilyConnectionUniversityId { get; set; }
    }
}
