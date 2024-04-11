using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CmdaFunding.Models
{
    [Table("UserMenuMapping")]
    public class UserMenuMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MappingId { get; set; }

        public int UserId { get; set; }

        public int MenuId { get; set; }

        [ForeignKey("UserId")]
        public UserMaster User { get; set; }

        [ForeignKey("MenuId")]
        public MenuMaster Menu { get; set; }
    }
}
