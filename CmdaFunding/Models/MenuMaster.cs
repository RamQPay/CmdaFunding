using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CmdaFunding.Models
{
    [Table ("MenuMaster")]
    public class MenuMaster
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuId { get; set; }

        [Required]
        [StringLength(30)]
        public string MenuName { get; set; }

        [Required]
        [StringLength(1)]
        public string MenuType { get; set; }

        [Required]
        [StringLength(100)]
        public string URL { get; set; }

        public byte? Sequence { get; set; }

        // Foreign key property
        [ForeignKey("ParentMenu")]
        public int? ParentMenuId { get; set; }

        // Navigation property
        public virtual MenuMaster ParentMenu { get; set; }
    }
}
