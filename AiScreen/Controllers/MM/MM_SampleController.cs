using Base.Model;
using BaseSQL.Interface;
using BaseWebApi.Interface;
using Dapper;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.MM;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Text.Encodings.Web;
using System.Web.Helpers;

namespace MaxSystemWebSite.Controllers.MM
{
    public class MM_SampleController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<MM_SampleController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;

        public MM_SampleController(ILogger<MM_SampleController> logger, IConfiguration configuration, IWebApi webApi,
           IDapper dapper, IAuthenticator authenticator, UserProfileService userProfileService, ISharePoint sharePoint)
       : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _userProfileService = userProfileService;
            _sharePoint = sharePoint;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FetchListing(string searchTerm = "", string tableName = "MM_SAMPLE", string columnsToSearch = "TITLE", int start = 0, int length = 10)
        {
            List<MM_SAMPLE> ListModel = new List<MM_SAMPLE>();
            int totalRecords = 0;

            string sortColumn = Request.Form[$"columns[{Request.Form["order[0][column]"]}][data]"];
            string sortDirection = Request.Form["order[0][dir]"]; // "asc" or "desc"

            try
            {
                var obj = new
                {
                    TableName = tableName,
                    SearchColumns = columnsToSearch,
                    SearchTerm = searchTerm,
                    Start = start,
                    Length = length,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection
                };

                // Create SQL connection
                using (var connection = new SqlConnection("Server=dbdev.azhamrosli.com,1433;Initial Catalog=DB_HRMS;Persist Security Info=False;User ID=devuser;Password=Black@654321;MultipleActiveResultSets=False;Encrypt=False;"))
                {
                    await connection.OpenAsync();

                    // Execute stored procedure with QueryMultiple
                    var result = await connection.QueryMultipleAsync(
                        "PSP_COMMON_LIST",
                        obj,
                        commandType: CommandType.StoredProcedure
                    );

                    // Read the paginated data
                    ListModel = result.Read<MM_SAMPLE>().ToList();

                    // Read the total record count as integer
                    totalRecords = result.ReadFirstOrDefault<int>();
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception or handle it accordingly
                // For now, we will not return the exception details for security reasons
            }

            return Json(new
            {
                data = ListModel,
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords
            });
        }


        public async Task<IActionResult> Detail(string ID = "")
        {
            MM_SAMPLE Model = new MM_SAMPLE();

            try
            {
                var obj = new
                {
                    ID_MM_SAMPLE = ID
                };
                (bool status, string message, MM_SAMPLE model) data = await _dapper.PSP_COMMON_DAPPER_SINGLE<MM_SAMPLE>("PSP_MM_SAMPLE", System.Data.CommandType.StoredProcedure, obj);
                if (data.status && data.model != null)
                {
                    Model = data.model;
                }
            }
            catch (Exception ex)
            {
                //await ErrorLog_Add_V3(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, UserID_Name);
            }

            return View(Model);
        }

        [HttpPost]
        public async Task<JsonResult> ExecuteSubmission(MM_SAMPLE Model)
        {
            var error = false;
            var msg = "Successfully saved data.";

            try
            {
                var param = new DynamicParameters();
                param.Add("@USER_ID", "");
                param.Add("@USER_IP", "");
                param.Add("@ID_MM_SAMPLE", Model.ID_MM_SAMPLE);
                param.Add("@TITLE", Model.TITLE);
                param.Add("@REMARK", Model.REMARK);
                (bool status, string message, ReturnSQL model) ReturnModel_Check = await _dapper.PSP_COMMON_DAPPER_SINGLE<ReturnSQL>("PSP_MM_SAMPLE_CHECK", System.Data.CommandType.StoredProcedure, param);
                if (ReturnModel_Check.status && ReturnModel_Check.model != null && ReturnModel_Check.model.Status != "NG")
                {
                    (bool status, string message, ReturnSQL model) ReturnModel = await _dapper.PSP_COMMON_DAPPER_SINGLE<ReturnSQL>("PSP_MM_SAMPLE_MAINT", System.Data.CommandType.StoredProcedure, param);
                    if (ReturnModel.status && ReturnModel.model != null && ReturnModel.model.Status != "NG")
                    {
                        //Success Action
                    }
                    else
                    {
                        error = true;
                        msg = (ReturnModel.model != null && !string.IsNullOrEmpty(ReturnModel.model.Opt1)) ? ReturnModel.model.Opt1 : "Failed to save data.";
                    }
                }
                else
                {
                    error = true;
                    msg = (ReturnModel_Check.model != null && !string.IsNullOrEmpty(ReturnModel_Check.model.Opt1)) ? ReturnModel_Check.model.Opt1 : "Error Occur.";
                }
            }
            catch (Exception ex)
            {
                //await ErrorLog_Add_V3(System.Reflection.MethodBase.GetCurrentMethod().Name, ex, UserID_Name);
                error = true;
                msg = ex.Message;
            }

            return Json(new { error = error, msg = msg });
        }


    }
}
