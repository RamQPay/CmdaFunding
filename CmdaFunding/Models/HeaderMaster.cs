using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmdaFunding.Models
{
    [Table("HeaderMaster")]
    public class HeaderMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte BudgetHeaderId { get; set; } 

        [Required]
        [StringLength(100)]
        public string HeaderName { get; set; }
    }
}
