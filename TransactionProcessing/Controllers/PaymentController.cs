using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TransactionProcessing.Models;
using System.Text;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.Metrics;
using System.Numerics;
using System.Reflection.PortableExecutable;

namespace TransactionProcessing.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IConfiguration configuration;
        public PaymentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> TransactionSave(TransactionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string mid = "";
                    string apiKey = "";
                    string apiSecret = "";
                    int headerId =0;
                    
                    //decimal amt = 500;

                    string connection = configuration.GetConnectionString("DefaultConnection");
                    using (SqlConnection con = new SqlConnection(connection))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("p_TransactionSave", con);   //Sp for storing data in Transaction-Table
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        cmd.Parameters.AddWithValue("@FileNumber", model.FileNumber);
                        cmd.Parameters.AddWithValue("@ChallanNumber", model.ChallanNumber);
                        cmd.Parameters.AddWithValue("@TransactionRefNo", model.TransactionRefNo);
                        cmd.Parameters.AddWithValue("@Amount", model.Amount);
                        cmd.Parameters.AddWithValue("@CustomerName", model.CustomerName);

                        //  for TransactionId
                        SqlParameter transactionIdParam = new SqlParameter("@TransactionId", SqlDbType.Int);
                        transactionIdParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(transactionIdParam);

                        cmd.ExecuteNonQuery();

                        int transactionId = (int)transactionIdParam.Value;

                        //--- Feedetails , BudgetHeaderId ----------  10/04/2024
                        foreach (var fee in model.Feedetails)
                        {
                            using (SqlConnection cons = new SqlConnection(connection))
                            {
                                cons.Open();

                                SqlCommand cmds = new SqlCommand("p_HeaderIdbyHeaderName", cons);
                                cmds.CommandType = CommandType.StoredProcedure;
                                cmds.Parameters.Add("@HeaderName", SqlDbType.VarChar).Value = fee.BudgetHeader;
                                using (SqlDataReader reader = cmds.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        headerId = Convert.ToInt32(reader["BudgetHeaderId"]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("No data found for the given BudgetHeaderName ID.");
                                    }

                                }
                                SqlCommand cmdheader = new SqlCommand("p_FeedetailsSave", con);
                                cmdheader.CommandType = CommandType.StoredProcedure;
                                cmdheader.Parameters.AddWithValue("@TransactionId", transactionId);
                                cmdheader.Parameters.AddWithValue("@BudgetHeaderId", headerId);
                                cmdheader.Parameters.AddWithValue("@Challanfee", fee.ChallanFee);
                                cmdheader.Parameters.AddWithValue("@PenalInterest", fee.PenalInterest);
                                cmdheader.ExecuteNonQuery();
                            }
                        }

                        //--- Feedetails , BudgetHeaderId ----------  10/04/2024


                        //decimal amount = model.Amount;                          //Doubt
                        int amountInPaisa = (int)model.Amount;
                        var requestData = new Dictionary<string, string>
                        {
                            { "amount", amountInPaisa.ToString() },
                            { "currency", "INR" },
                            { "receipt", transactionId.ToString() },                    // Use the transactionId as receipt
                            { "payment_capture", "1" }
                        };

                        //Sp for getting mid,keyid,keysecret from PaymentAccount based om TransactionId
                        SqlCommand command = new SqlCommand("p_GetPaymentGatewayInfoByTransactionId", con);  
                        command.CommandType = CommandType.StoredProcedure;                       
                        command.Parameters.Add("@TransactionId", SqlDbType.Int).Value = transactionId;                       

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                mid = reader["MID"].ToString();
                                apiKey = reader["keyid"].ToString();
                                apiSecret = reader["keysecret"].ToString();                                
                            }
                            else
                            {
                                Console.WriteLine("No data found for the given transaction ID.");
                            }
                        }

                        var baseUrl = "https://api.razorpay.com/v1/orders";

                        // Create HttpClient instance
                        using (var client = new HttpClient())
                        {
                            // Set Authorization header
                            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}")));
                            client.DefaultRequestHeaders.Authorization = authValue;

                            // Serialize requestData to JSON
                            var jsonContent = JsonConvert.SerializeObject(requestData);
                            
                            var response = await client.PostAsync(baseUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));
                           
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();

                                var responseObject = JsonConvert.DeserializeObject<dynamic>(content);
                                string orderId = responseObject.id;
                                SaveTransactionDetails(transactionId, orderId);
                                //return Ok(content);
                                //StringBuilder sb = new StringBuilder();
                                //sb.Append("<html>");
                                //sb.Append(@"<body onload='document.forms[""razorpayForm""].submit()'>"); 
                                //sb.Append("<form method ='POST' action ='https://api.razorpay.com/v1/checkout/embedded'>");
                                //sb.Append($"<input type='hidden' name='key_id' value='{apiKey}'/>"); 
                                //sb.Append($"<input type='hidden' name='order_id' value='{orderId}'/>"); 
                                ////sb.Append($"<input type='hidden' name='amount' value='{amountInPaisa}'/>"); 

                                //sb.Append($"<input type='hidden' name='amount' value='500 in paisa'/>"); 

                                //sb.Append("<input type='hidden' name='name' value='HDFC Collect Now'/>");
                                //sb.Append("<input type='hidden' name='description' value='Enter description'/>");
                                //sb.Append("<input type='hidden' name='prefill[email]' value='gaurav.kumar@example.com'/>");
                                //sb.Append("<input type='hidden' name='prefill[contact]' value='9999999999'/>");
                                //sb.Append($"<input type='hidden' name='notes[transaction_id]' value='{transactionId}'/>"); // Use the transactionId
                                //sb.Append("<input type='hidden' name='callback_url' value='RazorPay/CallBackUrl'/>"); // Provide your callback URL
                                //sb.Append("<button type='submit'>Submit</button>");
                                //sb.Append("</form>");
                                //sb.Append("</body>");
                                //sb.Append("</html>");
                                //string htmlContent = sb.ToString();
                                //return Content(htmlContent, "text/html");
                                
                                StringBuilder sb = new StringBuilder();
                                sb.Append("<html>");
                                sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
                                sb.AppendFormat("<form name='form' action='{0}' method='post'>", "https://api.razorpay.com/v1/checkout/embedded");
                                sb.AppendFormat("<input type='hidden' name='key_id' value='{0}'/>", apiKey);
                                sb.AppendFormat("<input type='hidden' name='order_id' value='{0}'/>", orderId);
                                sb.AppendFormat("<input type='hidden' name='amount' value='{0}'/>",amountInPaisa);
                                sb.AppendFormat("<input type='hidden' name='name' value='HDFC Collect Now'/>");
                                sb.AppendFormat("<input type='hidden' name='prefill[email]' value='gaurav.kumar@example.com'/>");
                                sb.AppendFormat("<input type='hidden' name='prefill[contact]' value='9999999999'/>");
                                sb.AppendFormat($"<input type='hidden' name='notes[transaction_id]' value='{transactionId}'/>");
                                sb.AppendFormat("<input type='hidden' name='callback_url' value='https://localhost:7179/Razorpay/CallbackUrl'/>");

                                // Apply CSS to hide the button
                                sb.Append("<style type='text/css'>");
                                sb.Append("button[type='submit'] { display: none; }");
                                sb.Append("</style>");

                                // Add the submit button
                                sb.AppendFormat("<button id='submit-button' type='submit'>Submit</button>");
                                sb.Append("</form>");
                                sb.Append("</body>");
                                sb.Append("</html>");

                                //return Ok(sb.ToString());

                                string htmlContent = sb.ToString();
                                return Content(htmlContent, "text/html");


                                //Response.Write(sb.ToString());
                                //HttpContext.Current.Response.End();
                                //return;


                            }
                            else
                            {
                                return StatusCode((int)response.StatusCode, "Failed to create order in Razorpay.");
                            }
                        }
                    }
                }
                else
                {
                    return Content("Invalid Transaction Details");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private void SaveTransactionDetails(int transactionId, string orderId) //Updating orderid in TransactionTable 
        {
            string connection = configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("p_UpdateOrderId", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Add parameters
                cmd.Parameters.AddWithValue("@TransactionId", transactionId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
    }
}
