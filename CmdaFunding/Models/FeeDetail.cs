using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CmdaFunding.Models
{
    [Table("FeeDetail")]
    public class FeeDetail
    {
        public int TransactionId { get; set; }
        public int BudgetHeaderId { get; set; }
        public string Challanfee { get; set; }
        public string PenalInterest { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("TransactionId")]
        public Transactions Transactions { get; set; }

        [ForeignKey("BudgetHeaderId")]
        public HeaderMaster HeaderMaster { get; set; }
    }
}
