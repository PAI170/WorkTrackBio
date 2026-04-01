using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkTrackBio.API.Data.Common;

namespace WorkTrackBio.API.Data.Models
{
    public class ChristmasBonusDetail : BaseEntity
    {
        [Required]
        public int ChristmasBonusId { get; set; }
        [ForeignKey("ChristmasBonusId")]
        public virtual ChristmasBonus ChristmasBonus { get; set; } = null!;

        [Required]
        public int PayrollEntryId { get; set; }
        [ForeignKey("PayrollEntryId")]
        public virtual PayrollEntry PayrollEntry { get; set; } = null!;

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal EarnedAmount { get; set; }
    }
}
