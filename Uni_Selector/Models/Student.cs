using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [StringLength(100)]
        public string Province { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(200)]
        public string? Area { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [Required]
        [Range(0, 100)]
        public double GPA { get; set; }

        [Required]
        public Enums.PathType Path { get; set; }

        [StringLength(100)]
        public string? Track { get; set; } 
        public bool BtecLevel2Completed { get; set; }
        public bool BtecLevel3Completed { get; set; }

        [StringLength(500)]
        public string? BtecCertificateUrl { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RegistrationBudget { get; set; }

        [StringLength(500)]
        public string? DesiredMajors { get; set; } 

        [StringLength(100)]
        public string? PreferredCity { get; set; }

        public int MaxDistanceKm { get; set; } = 50;

        public LanguageCode PreferredLanguage { get; set; } = LanguageCode.English;

        public bool HasFamilyConnection { get; set; }

        public int? FamilyConnectionUniversityId { get; set; }

        [ForeignKey("FamilyConnectionUniversityId")]
        public University? FamilyConnectionUniversity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


        public ICollection<Recommendation> Recommendations { get; set; }
        public ICollection<StudentApplication> Applications { get; set; }
    }
}

