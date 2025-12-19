namespace Uni_Selector.ViewModels.Student
{
    public class StudentDetailsViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime MemberSince { get; set; }
        public DateTime? LastLogin { get; set; }

        // Personal Information
        public string NationalId { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; } = string.Empty;

        // Location
        public string Province { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string? Area { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Academic Information
        public double GPA { get; set; }
        public string Path { get; set; } = string.Empty;
        public string? AcademicTrack { get; set; }
        public string? VocationalBranch { get; set; }
        public bool BtecLevel2Completed { get; set; }
        public bool BtecLevel3Completed { get; set; }
        public string? BtecCertificateUrl { get; set; }

        // Preferences
        public decimal RegistrationBudget { get; set; }
        public string? DesiredMajors { get; set; }
        public string? PreferredCity { get; set; }
        public int MaxDistanceKm { get; set; }
        public string PreferredLanguage { get; set; } = string.Empty;

        // Family Connection
        public bool HasFamilyConnection { get; set; }
        public string? FamilyConnectionUniversityName { get; set; }

        // Guardian Information
        public string GuardianName { get; set; } = string.Empty;
        public string GuardianPhone { get; set; } = string.Empty;
        public string GuardianRelation { get; set; } = string.Empty;

        // Special Circumstances
        public bool HasDisability { get; set; }
        public string? DisabilityType { get; set; }
        public bool IsOrphan { get; set; }
        public bool IsEmployeeChild { get; set; }

        // Statistics
        public int TotalRecommendations { get; set; }
        public int TotalApplications { get; set; }
    }
}
