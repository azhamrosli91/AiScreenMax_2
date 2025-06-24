using Base.Model;
using BaseSQL.Interface;
using BaseWebApi.Interface;
using Dapper;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSys.Models.DE;
using MaxSystemWebSite.Controllers.DE;
using MaxSystemWebSite.Models.DE;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.SETTING;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace AiScreen.Controllers.DE
{
    public class PaymentController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<PaymentController> _logger;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper _dapper;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IEmail _emailService;
        private readonly IConfiguration _configuration;

        public PaymentController(ILogger<PaymentController> logger,
            IConfiguration configuration,
            IWebApi webApi,
            IDapper dapper,
            IAuthenticator authenticator,
            UserProfileService userProfileService,
            ISharePoint sharePoint,
            IHttpClientFactory clientFactory,
            IEmail emailService
        ) : base(configuration, webApi, dapper, authenticator)
        {
            _logger = logger;
            _dapper = dapper;
            _userProfileService = userProfileService;
            _sharePoint = sharePoint;
            _clientFactory = clientFactory;
            _emailService = emailService;
            _configuration = configuration;
        }


        // POST: /Payment/CreateBill
        [HttpPost]
        public async Task<IActionResult> CreateBill([FromBody] ToyyibPayBillRequest request)
        {
            try
            {
                string userId = USER_ID;
                string userEmail = EMAIL;
                int validity = int.Parse(request.Validity);
                string amount = request.Amount.Replace(".", "");

                var client = _clientFactory.CreateClient();

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("userSecretKey", _configuration["ToyyibPay:SecretKey"]),
                    new KeyValuePair<string, string>("categoryCode", _configuration["ToyyibPay:CategoryCode"]),
                    new KeyValuePair<string, string>("billName", "AiScreen Pro Plan Subscription"),
                    new KeyValuePair<string, string>("billDescription", $"{validity}-Month Subscription Plan"),
                    new KeyValuePair<string, string>("billPriceSetting", "1"),
                    new KeyValuePair<string, string>("billPayorInfo", "1"),
                    new KeyValuePair<string, string>("billAmount", amount),
                    new KeyValuePair<string, string>("billReturnUrl", $"{_configuration["AppUrl"]}/Resume/Subscription?status={{status}}"),
                    new KeyValuePair<string, string>("billCallbackUrl", $"{_configuration["AppUrl"]}/Payment/ToyyibPayCallback"),
                    new KeyValuePair<string, string>("billExternalReferenceNo", $"INV{DateTime.Now.ToString("yyMMddHHmmss")}"),
                    new KeyValuePair<string, string>("billTo", userEmail),
                    new KeyValuePair<string, string>("billEmail", userEmail),
                    new KeyValuePair<string, string>("billPhone", "0172210940"),
                });

                var response = await client.PostAsync("https://toyyibpay.com/index.php/api/createBill", content);
                var responseString = await response.Content.ReadAsStringAsync();

                var json = JArray.Parse(responseString);
                var billCode = json[0]["BillCode"].ToString();
                var billUrl = $"https://toyyibpay.com/{billCode}";

                return Json(new { success = true, billUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create ToyyibPay bill.");
                return Json(new { success = false });
            }
        }
        //[HttpPost]
        //public async Task<IActionResult> ToyyibPayCallback([FromForm] IFormCollection form)
        //{
        //    try
        //    {
        //        string billCode = form["billCode"];
        //        string billpaymentStatus = form["billpaymentStatus"]; // 1 = Success, 2 = Failed

        //        bool isSuccess = billpaymentStatus == "1";
        //        string userId = USER_ID;
        //        string email = EMAIL;

        //        // You can extract validity from billDescription if needed
        //        int validity = 1; // Default 1 month; ideally, fetch from billCode or external ref
        //        DateTime start = DateTime.Now;
        //        DateTime end = isSuccess ? start.AddMonths(validity) : DateTime.MinValue;

        //        var model = new DE_SUBSCRIPTIONModel
        //        {
        //            USER_ID = userId,
        //            USER_EMAIL = email,
        //            STATUS = isSuccess,
        //            TYPE_PLAN = validity,
        //            START_SUBCRIPT_DATE = start,
        //            END_SUBCRIPT_DATE = end,
        //            OPT_MSG = isSuccess ? "Payment Successful" : "Payment Failed"
        //        };

        //        var parameters = new DynamicParameters();
        //        parameters.Add("USER_ID", model.USER_ID);
        //        parameters.Add("USER_EMAIL", model.USER_EMAIL);
        //        parameters.Add("STATUS", model.STATUS);
        //        parameters.Add("TYPE_PLAN", model.TYPE_PLAN);
        //        parameters.Add("START_SUBCRIPT_DATE", model.START_SUBCRIPT_DATE);
        //        parameters.Add("END_SUBCRIPT_DATE", model.END_SUBCRIPT_DATE);
        //        parameters.Add("OPT_MSG", model.OPT_MSG);

        //        using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //        {
        //            await connection.ExecuteAsync("PSP_DE_SUBCRIPTION_MAINT", parameters, commandType: CommandType.StoredProcedure);
        //        }

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "ToyyibPay Callback error");
        //        return StatusCode(500);
        //    }
        //}

        //        // GET: /DE/Payment/ToyyiBPaySuccess
        //        public async Task<IActionResult> ToyyiBPaySuccess(string status_id, string billcode, string order_id, string msg, string transaction_id)
        //        {
        //            _logger.LogInformation($"ToyyibPay return: status_id={status_id}, order_id={order_id}, msg={msg}");

        //            switch (status_id)
        //            {
        //                case "1":
        //                    return View("ToyyiBPaySuccess"); // show success view
        //                case "2":
        //                case "3":
        //                    return View("ToyyiBPayFail");
        //                default:
        //                    // optional: pass a query param to show message on checkout
        //                    return Redirect($"/Resume/Subscription?payment=failed");
        //            }
        //        }

        //        // GET: /DE/Payment/ToyyiBPayFail
        //        public async Task<IActionResult> ToyyiBPayFail()
        //        {
        //            return View();
        //        }

        //private int DetectMonthFromAmount(string amount)
        //{
        //    var cleanAmount = amount.Replace(".", "").Trim();

        //    return cleanAmount switch
        //    {
        //        "4999" => 1,
        //        "52900" => 12,
        //        "149900" => 36,
        //        _ => 1
        //    };
        //}
    }

    }
