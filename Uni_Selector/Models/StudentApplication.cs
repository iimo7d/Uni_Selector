using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Uni_Selector.Models.Enums;

namespace Uni_Selector.Models
{
    public class StudentApplication
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public int? UniversityProgramId { get; set; }

        [ForeignKey("UniversityProgramId")]
        public UniversityProgram? UniversityProgram { get; set; }

        public int? BtecProgramId { get; set; }

        [ForeignKey("BtecProgramId")]
        public BtecProgram? BtecProgram { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovalDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        // University rep can set hour discount between 30-50%
        [Range(30, 50)]
        public decimal? HourDiscountPercent { get; set; }

        // Fixed: Added proper FK attribute
        public string? HourDiscountSetByUserId { get; set; }

        [ForeignKey("HourDiscountSetByUserId")]
        public ApplicationUser? HourDiscountSetByUser { get; set; }

        public DateTime? HourDiscountSetAt { get; set; }

        // For commission mode 3 calculation
        [Range(0, 60)] // Increased max to 60 to accommodate heavier first semesters
        public int? PlannedFirstSemesterHours { get; set; }

        public DiscountGrant? DiscountGrant { get; set; }
        public Commission? Commission { get; set; }
    }
}
