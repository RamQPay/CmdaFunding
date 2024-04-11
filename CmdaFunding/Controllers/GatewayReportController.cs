using CmdaFunding.Data;
using CmdaFunding.Models;
using CmdaFunding.Repositories.Implementation;
using CmdaFunding.Repositories.Interface;
using CmdaFunding.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Nest;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using TechTalk.SpecFlow.CommonModels;

namespace CmdaFunding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GatewayReportController : ControllerBase
    {
        private readonly IGatewayReportRepository _gatewayreportRepository;                   
        public GatewayReportController(IGatewayReportRepository gatewayreportRepository)
        {
            _gatewayreportRepository = gatewayreportRepository;
        }
                                    //Transaction-Report

        [HttpGet("GatewayNames")]
        public async Task<IEnumerable<string>> GetAllGatewayNames()                     //List of Gateway Names -DropDown
        {
            return await _gatewayreportRepository.GetAllGatewayNames();
        }

        [HttpGet("TransactionReportSave")]
        public async Task<List<Transactions>> TransactionReportSave(DateTime fromdate,DateTime todate,string gateway)
        {
            return await _gatewayreportRepository.TransactionReportSave(fromdate, todate, gateway);
        }

                                    //Header-Report

        [HttpGet("HeaderNames")]
        public async Task<IEnumerable<string>> GetAllHeaderNames()                    //List Of Header Names -DropDown
        {
            return await _gatewayreportRepository.GetAllHeaderNames();
        }
        
    }
}
