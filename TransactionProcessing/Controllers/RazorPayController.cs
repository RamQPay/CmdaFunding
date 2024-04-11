using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;
using TransactionProcessing.Models;

namespace TransactionProcessing.Controllers
{
    public class RazorPayController : Controller
    {
        private readonly IConfiguration configuration;
        public RazorPayController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CallbackUrl(string razorpay_order_id, string razorpay_payment_id, string razorpay_signature)
        {
            RayzorPayViewModel rz = new RayzorPayViewModel();
            rz.razorpay_order_id = razorpay_order_id;
            rz.razorpay_payment_id = razorpay_payment_id;
            rz.razorpay_signature = razorpay_signature;


            string mid = "";
            string apiKey = "";
            string apiSecret = "";
            

            string merchantOrderID = "";
            string mSPReferenceID = "";
            int responsecode=0;
            string message="";

            try
            {
                string connection = configuration.GetConnectionString("DefaultConnection");

                // Use SqlConnection and SqlCommand to fetch data from the database
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("p_GetPaymentGatewayInfoByRazorpay_order_id", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Api_OrderId", SqlDbType.VarChar).Value = razorpay_order_id;


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
                                Console.WriteLine("No data found for the given RazorPay_Order ID.");
                            }
                        }
                    }
                }

                // Prepare baseUrl for Razorpay API
                var baseUrl = $"https://api.razorpay.com/v1/payments/{razorpay_payment_id}";

                // Create HttpClient instance
                using (var client = new HttpClient())
                {
                    // Set Authorization header
                    var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:{apiSecret}")));
                    client.DefaultRequestHeaders.Authorization = authValue;
                    var response = await client.GetAsync(baseUrl);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(content);
                        string statuschecks = responseObject.status;
                        UpdateTransactionStatusCapture(razorpay_order_id, statuschecks);
                        //return Ok(content);
                        

                    }
                    else
                    {
                        return StatusCode((int)response.StatusCode, "Failed to fetch payment details from Razorpay.");
                    }
                }

                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand("p_CmdaCallBack", con))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@Api_OrderId", SqlDbType.VarChar).Value = razorpay_order_id;


                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                merchantOrderID = reader["CMDAOrderId"].ToString();
                                mSPReferenceID = reader["TransactionId"].ToString();
                                var sts = reader["Status"].ToString();
                                if(sts=="2")
                                {
                                    responsecode = 200;
                                    message = "Payment success";
                                }
                                if (sts == "3")
                                {
                                    responsecode = 201;
                                    message = "Payment failed";
                                }
                            }
                            else
                            {
                                Console.WriteLine("No data found for the given RazorPay_Order ID.");
                            }
                        }
                    }
                }


                // Make an HTTP POST request to the CMDACallbackUrl action
                var cmdaCallbackUrl = "https://localhost:7179/CMDACallBack/CMDACallbackUrl"; // Replace with your actual URL
                using (var client = new HttpClient())
                {
                    var data = new Dictionary<string, string>
                    {
                        { "merchantOrderID", merchantOrderID },
                        { "mSPReferenceID", mSPReferenceID },
                        { "responsecode", responsecode.ToString() },
                        { "message", message }
                    };

                    var content = new FormUrlEncodedContent(data);
                    var response = await client.PostAsync(cmdaCallbackUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        //return Ok(data);
                    }
                    else
                    {
                        
                    }
                    return Ok(data);
                }
                
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            
            

            
            
        }

        public void UpdateTransactionStatusCapture(string razorpay_order_id,string statuschecks)
        {
            string connection = configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("p_UpdateTransactionStatusCapture", con);
                cmd.CommandType = CommandType.StoredProcedure; 
                cmd.Parameters.AddWithValue("@Api_OrderId", razorpay_order_id);
                cmd.Parameters.AddWithValue("@Status", statuschecks);
                cmd.ExecuteNonQuery();
                con.Close();
            }

        }
        

    }
}