using CmdaFunding.Commons.Enums;
using CmdaFunding.Controllers;
using CmdaFunding.Data;
using CmdaFunding.Models;
using CmdaFunding.Repositories.Interface;
using CmdaFunding.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using TechTalk.SpecFlow.CommonModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmdaFunding.Repositories.Implementation
{
    public class GatewayReportRepository : IGatewayReportRepository
    {
        private FundingContext _dbContext;
        public GatewayReportRepository(FundingContext dbContext)
        {
            _dbContext = dbContext;
        }

        //--------------------------------Transaction - Report----------------------------------
        public async Task<IEnumerable<string>> GetAllGatewayNames()
        {
            var gatewayNames = await _dbContext.PaymentGatewayMaster.Select(i=>i.PaymentGatewayname).ToListAsync();
            return gatewayNames;
        }

        public async Task<List<Transactions>>TransactionReportSave(DateTime fromdate, DateTime todate, string gateway)
        {
            try 
            {
                List<Transactions> trs = new List<Transactions>();
                var toDateAdjusted = todate.Date.AddDays(1).AddMilliseconds(-1); // Adjust the 'todate' to include the entire day

                byte? gatewayId = null;
                var gatewayMaster = _dbContext.PaymentGatewayMaster.Where(i => i.PaymentGatewayname == gateway).FirstOrDefault();
                if (gatewayMaster != null)
                {
                    gatewayId = gatewayMaster.PaymentGatewayid;
                }

                var transReport = _dbContext.Transactions
                                  .Where(i => i.TransactionDate >= fromdate && i.TransactionDate <= toDateAdjusted && i.PaymentGatewayId == gatewayId)
                                  .ToList();
                transReport = transReport.OrderBy(i => i.TransactionId).ToList();

                foreach (var t in transReport)
                {
                    Transactions tr = new Transactions();
                    {
                        tr.TransactionId = t.TransactionId;
                        tr.TransactionDate = t.TransactionDate;
                        tr.PaymentGatewayId = t.PaymentGatewayId;
                        tr.CMDAOrderId = t.CMDAOrderId;
                        tr.PaymentGatewayAccountId = t.PaymentGatewayAccountId;
                        tr.TransactionAmount = t.TransactionAmount;
                        tr.TransactionType = t.TransactionType;
                        tr.Status = t.Status;
                        tr.CustomerName = t.CustomerName;
                        tr.FileNumber = t.FileNumber;
                        tr.ChallanNumber = t.ChallanNumber;
                        tr.Api_OrderId = t.Api_OrderId;
                    }

                    trs.Add(tr); 
                }
                return trs;
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        //--------------------------------Header - Report----------------------------------
        public async Task<IEnumerable<string>> GetAllHeaderNames()
        {
            var headersList = await _dbContext.HeaderMaster.Select(i=>i.HeaderName).ToListAsync();
            return headersList;
        }
    }
}
