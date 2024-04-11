using Microsoft.AspNetCore.Mvc;
using TransactionProcessing.Models;

namespace TransactionProcessing.Controllers
{
    public class CMDACallBackController : Controller
    {
        private readonly IConfiguration configuration;
        public CMDACallBackController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CMDACallbackUrl(string merchantOrderID,string mSPReferenceID, int responsecode, string message)
        {
            CMDACallBackViewModel cmd  = new CMDACallBackViewModel();
            cmd.MerchantOrderID = merchantOrderID;
            cmd.MSPReferenceID = mSPReferenceID;
            cmd.ResponseCode = responsecode;    
            cmd.Message = message;  
            return Ok(cmd);
        }
    }
}
