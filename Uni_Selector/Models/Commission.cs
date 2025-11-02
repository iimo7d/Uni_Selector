using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uni_Selector.Models
{
    public class Commission
    {
        public int Id { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        [ForeignKey("ApplicationId")]
        public StudentApplication Application { get; set; }

        [Required]
        [Range(0, 100)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Percentage { get; set; } = 2m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountEstimated { get; set; }

        public bool Settled { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? MonthlySettlementId { get; set; }

        [ForeignKey("MonthlySettlementId")]
        public MonthlySettlement? MonthlySettlement { get; set; }
    }
}
