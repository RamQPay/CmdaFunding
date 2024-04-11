using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmdaFunding.Models
{
    [Table("Transactions")]
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public byte? PaymentGatewayId { get; set; }
        public string CMDAOrderId { get; set; }
        public byte? PaymentGatewayAccountId { get; set; }
        public decimal? TransactionAmount { get; set; }
        public byte? TransactionType { get; set; }
        public byte? Status { get; set; }
        public string CustomerName { get; set; }
        public string FileNumber { get; set; }
        public string ChallanNumber { get; set; }
        public string Api_OrderId { get; set; }

        //[ForeignKey("PaymentGatewayId")]
        //public PaymentGatewayMaster PaymentGatewayMaster { get; set; }

        //[ForeignKey("PaymentGatewayAccountId")]
        //public PaymentGatewayAccount PaymentGatewayAccount { get; set; }

    }
}
