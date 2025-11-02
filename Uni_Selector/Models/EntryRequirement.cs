using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;

namespace Uni_Selector.Models
{
    public class EntryRequirement
    {
        public int Id { get; set; }

        [Required]
        public int UniversityProgramId { get; set; }

        [ForeignKey("UniversityProgramId")]
        public UniversityProgram UniversityProgram { get; set; }

        [Required]
        [Range(0, 100)]
        public double MinGPA { get; set; }

        [Required]
        public PathType Path { get; set; }

        [StringLength(100)]
        public string? Track { get; set; } 

        public bool AllowNoTawjihi { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
