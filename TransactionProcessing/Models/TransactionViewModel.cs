namespace TransactionProcessing.Models
{
    public class TransactionViewModel
    {
        public string FileNumber {  get; set; }
        public string ChallanNumber { get; set; }
        public string TransactionRefNo { get; set; }
        public decimal Amount { get; set; }
        public string CustomerName { get; set; }
        public List<FeedetailViewModel> Feedetails { get; set; }
    }

    public class FeedetailViewModel
    {
        public string BudgetHeader { get; set; }
        public string ChallanFee { get; set; }
        public string PenalInterest { get; set; }
    }
}