using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmdaFunding.Models
{
    [Table("PaymentGatewayMaster")]
    public class PaymentGatewayMaster
    {
        [Key]
        public byte PaymentGatewayid { get; set; } 

        [Required]
        [StringLength(50)]
        public string PaymentGatewayname { get; set; }
    }
}
