using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmdaFunding.Models
{
    [Table("UserMaster")]
    public class UserMaster
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
