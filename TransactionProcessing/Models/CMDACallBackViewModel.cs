namespace TransactionProcessing.Models
{
    public class CMDACallBackViewModel
    {
        public string MerchantOrderID { get; set; }
        public string MSPReferenceID { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
    }
}
