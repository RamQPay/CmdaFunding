using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CmdaFunding.Models
{
    [Table("PaymentGatewayAccount")]
    public class PaymentGatewayAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte PaymentGatewayAccountid { get; set; }
        public byte? PaymentGatewayMasterid { get; set; }

        [StringLength(20)]
        public string AccountName { get; set; }

        [StringLength(20)]
        public string MID { get; set; }

        [StringLength(30)]
        public string keyid { get; set; }

        [StringLength(30)]
        public string keysecret { get; set; }

        [ForeignKey("PaymentGatewayMasterid")]
        public PaymentGatewayMaster PaymentGatewayMaster { get; set; }
    }
}
