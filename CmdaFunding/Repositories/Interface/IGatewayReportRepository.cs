
using CmdaFunding.Models;
using CmdaFunding.ViewModels;
using Microsoft.AspNetCore.Mvc;
using TechTalk.SpecFlow.CommonModels;

namespace CmdaFunding.Repositories.Interface
{
    public interface IGatewayReportRepository
    {
        Task<IEnumerable<string>> GetAllGatewayNames();
        Task<List<Transactions>> TransactionReportSave(DateTime fromdate, DateTime todate, string gateway);
        Task<IEnumerable<string>> GetAllHeaderNames();
    }
}
