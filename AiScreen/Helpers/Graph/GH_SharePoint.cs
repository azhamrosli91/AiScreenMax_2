using Azure.Identity;
using Microsoft.Graph.Models;
using Microsoft.Graph;
using MaxSys.Interface;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.SETTING;
using E_Template.Helpers;
using Microsoft.Extensions.Options;
using static Dapper.SqlMapper;
using Org.BouncyCastle.Crypto.Agreement;
using MaxSys.Models;
using Microsoft.Graph.Models.TermStore;
using System.Diagnostics;
using Org.BouncyCastle.Ocsp;
using iText.StyledXmlParser.Jsoup.Nodes;

namespace MaxSystemWebSite.Helpers.Graph
{
    public class GH_SharePoint :  ISharePoint
    {

        // Client secret credential
        private static ClientSecretCredential? _clientSecretCredential;
        // Client configured with application authentication
        private static GraphServiceClient? _appClient;
        public readonly IConfiguration _configuration;

        public GH_SharePoint(IConfiguration configuration)
        {
            _configuration = configuration;


            if (string.IsNullOrEmpty(_configuration["Settings:TenantId"]) ||
               string.IsNullOrEmpty(_configuration["Settings:ClientId"]) ||
               string.IsNullOrEmpty(_configuration["Settings:ClientSecret"]) || 
               string.IsNullOrEmpty(_configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0]))
            {
                throw new ArgumentException("Invalid Graph API settings");
            }

            // Initialize the credential using client secret
            _clientSecretCredential = new ClientSecretCredential(
            _configuration["Settings:TenantId"],
            _configuration["Settings:ClientId"],
                _configuration["Settings:ClientSecret"]
            );

            // Initialize Graph client with the credential and desired scopes
            _appClient = new GraphServiceClient(_clientSecretCredential, new[] { _configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0] });

        }
        //public GraphHelper(IOptions<Settings> settings)
        //{
        //    _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        //    // Ensure that required properties are not null
        //    if (string.IsNullOrEmpty(_settings.TenantId) ||
        //        string.IsNullOrEmpty(_settings.ClientId) ||
        //        string.IsNullOrEmpty(_settings.ClientSecret))
        //    {
        //        throw new ArgumentException("Invalid Graph API settings");
        //    }

        //    // Initialize the credential using client secret
        //    var clientSecretCredential = new ClientSecretCredential(
        //        _settings.TenantId,
        //        _settings.ClientId,
        //        _settings.ClientSecret
        //    );

        //    // Initialize Graph client with the credential and desired scopes
        //    _appClient = new GraphServiceClient(clientSecretCredential, _settings.GraphUserScopes);
        //}

        public void InitGraph(SETTING_EMAIL settings)
        {

            // Ensure that required properties are not null
            if (string.IsNullOrEmpty(settings.TENANT_ID) ||
                string.IsNullOrEmpty(settings.CLIENT_ID) ||
                string.IsNullOrEmpty(settings.CLIENT_SECRET))
            {
                throw new ArgumentException("Invalid Graph API settings");
            }

            // Initialize the credential using client secret
            _clientSecretCredential = new ClientSecretCredential(
                settings.TENANT_ID,
                settings.CLIENT_ID,
                settings.CLIENT_SECRET
            );

            // Initialize Graph client with the credential and desired scopes
            _appClient = new GraphServiceClient(_clientSecretCredential, new[] { settings.GRAPH_USER });
        }

        #region "SP Employee Information"
        public async Task<(bool success, string message, ListCollectionResponse? data)> GetList(string _siteID)
        {
            try
            {
                _ = _appClient ??
                        throw new NullReferenceException("Graph has not been initialized for app auth");

                var lists = await _appClient.Sites[_siteID].Lists
                              .GetAsync();

                return (true, "OK", lists);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }

        }

        public async Task<(bool success, string message, SP_EmployeeInformation data)> GetEmployeeInformationDetail(string _siteID, string listID, string email)
        {
            try
            {
                _ = _appClient ?? throw new NullReferenceException("Graph has not been initialized for app auth");

                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                });

                foreach (var record in recordList.Value)
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;

                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        DateTime? GetDate(string key)
                        {
                            if (data.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var dt))
                            {
                                return dt.ToLocalTime();
                            }
                            return null;
                        }
                        bool GetBool(string key) => bool.TryParse(GetString(key), out var v) ? v : false;

                        int GetEmpId()
                        {
                            if (data.TryGetValue("EmployeeID", out var val) && int.TryParse(val?.ToString(), out var id))
                            {
                                return id;
                            }
                            return 0;
                        }

                        string emailFromList = GetString("field_1"); // assuming field_6 holds the email

                        if (!string.IsNullOrEmpty(emailFromList) && emailFromList.Equals(email, StringComparison.OrdinalIgnoreCase))
                        {
                            string empId = record.AdditionalData.TryGetValue("@odata.etag", out var etagValue) ? etagValue?.ToString() : "";
                            string photoUrl = GetString("EmployeeImage");

                            var item = new SP_EmployeeInformation
                            {
                                EMP_ID = empId,
                                EMP_NAME = GetString("Title"),
                                EMP_EMAIL = GetString("field_1"),
                                EMP_NO = GetString("field_2"),
                                DAY_JOINED = GetInt("Days_x0020_Joined"),
                                DESIGNATION = GetString("field_7"),
                                DEPARTMENT = GetString("field_9"),
                                JOIN_DATE = GetDate("field_11"),
                                TMS_MANAGER = GetString("TMSManager"),
                                TEAM = GetString("Team"),
                                LEADER = GetString("Leader"),
                                RESIGNATION_DATE = GetDate("TMSManager"),
                                LAST_WORKING_DATE = GetDate("TMSManager"),
                                BIRTH_DATE = GetDate("field_14"),
                                EMP_PHOTO = null,
                                EMP_STATUS = GetString("field_6"),
                                GENDER = GetString("field_15"),
                                RACE = GetString("field_16"),
                                MARITAL_STATUS = GetString("field_17"),
                                HIGH_EDUCATION = GetString("field_18"),
                                LEAVE_ANUAL = GetInt("AnnualLeaveEntitlement"),
                                FORECAST_TOTAL_DAYS = GetInt("ForecastTotalDays"),
                                SURVEY_SUBMISSION = GetBool("field_21"),
                            };

                            return (true, "OK", item);
                        }
                    }
                }

                return (false, "Employee not found", null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool success, string message, List<SP_EmployeeInformation> data)> GetEmployeeInformation(string _siteID, string listID)
        {
            try
            {
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");
                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                });

                List<SP_EmployeeInformation> dataModel = new List<SP_EmployeeInformation>();

                foreach (var record in recordList.Value)
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;

                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        DateTime? GetDate(string key)
                        {
                            if (data.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var dt))
                            {
                                return dt.ToLocalTime();
                            }
                            return null;
                        }
                        bool GetBool(string key) => bool.TryParse(GetString(key), out var v) ? v : false;

                        int GetEmpId()
                        {
                            if (data.TryGetValue("EmployeeID", out var val) && int.TryParse(val?.ToString(), out var id))
                            {
                                return id;
                            }
                            return 0;
                        }

                        // Get photo URL
                        string photoUrl = GetString("EmployeeImage");



                        // 🛠 Get ETag as EMP_ID
                        string empId = record.AdditionalData.TryGetValue("@odata.etag", out var etagValue) ? etagValue?.ToString() : "";


                        var item = new SP_EmployeeInformation
                        {
                            EMP_ID = empId,
                            EMP_NAME = GetString("Title"),
                            EMP_EMAIL = GetString("field_1"),
                            EMP_NO = GetString("field_2"),
                            DAY_JOINED = GetInt("Days_x0020_Joined"),
                            DESIGNATION = GetString("field_7"),
                            DEPARTMENT = GetString("field_9"),
                            JOIN_DATE = GetDate("field_11"),
                            TMS_MANAGER = GetString("TMSManager"),
                            TEAM = GetString("Team"),
                            LEADER = GetString("Leader"),
                            RESIGNATION_DATE = GetDate("TMSManager"),
                            LAST_WORKING_DATE = GetDate("TMSManager"),
                            BIRTH_DATE = GetDate("field_14"),
                            EMP_PHOTO = null,
                            EMP_STATUS = GetString("field_6"),
                            GENDER = GetString("field_15"),
                            RACE = GetString("field_16"),
                            MARITAL_STATUS = GetString("field_17"),
                            HIGH_EDUCATION = GetString("field_18"),
                            LEAVE_ANUAL = GetInt("AnnualLeaveEntitlement"),
                            FORECAST_TOTAL_DAYS = GetInt("ForecastTotalDays"),
                            SURVEY_SUBMISSION = GetBool("field_21"),
                        };

                        dataModel.Add(item);
                    }
                }


                return (true, "OK", dataModel);
            }
            catch (Exception ex)
            {

                return (false, ex.Message, null);
            }

        }
        #endregion
        #region"Leave Information"
        public async Task<(bool success, string message, List<SP_LeaveHistory> data)> GetLeaveHistory(string _siteID, string listID, string email)
        {
            try
            {
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");
                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                    requestConfiguration.QueryParameters.Orderby = ["fields/Modified desc"];
                });

                List<SP_LeaveHistory> dataModel = new List<SP_LeaveHistory>();

                foreach (var record in recordList.Value) 
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;
                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        decimal GetDecimal(string key) => decimal.TryParse(GetString(key), out var v) ? v : 0.0m;

                        DateTime? GetDate(string key)
                        {
                            if (data.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var dt))
                            {
                                return dt.ToLocalTime();
                            }
                            return null;
                        }
                        bool GetBool(string key) => bool.TryParse(GetString(key), out var v) ? v : false;

                        int GetEmpId()
                        {
                            if (data.TryGetValue("EmployeeID", out var val) && int.TryParse(val?.ToString(), out var id))
                            {
                                return id;
                            }
                            return 0;
                        }

                        string emailFromList = GetString("Email"); // assuming field_6 holds the email

                        if (!string.IsNullOrEmpty(emailFromList) && emailFromList.Equals(email, StringComparison.OrdinalIgnoreCase)) 
                        {
                            var item = new SP_LeaveHistory
                            {
                                EMP_NAME = GetString("Title"),
                                EMP_EMAIL = GetString("Email"),
                                EMP_NO = GetString("field_1"),
                                DATE_START = GetDate("field_2"),
                                DATE_END = GetDate("field_3"),
                                NUMBER_OF_LEAVE = GetDecimal("field_4"),
                                TYPE_LEAVE = GetString("field_5"),
                                APPROVER = GetString("field_6"),
                                APPROVER_DATE = GetDate("field_7"),
                                EMERGENCY_LEAVE = GetString("ContentType"),
                            };

                            dataModel.Add(item);
                        }

                    }

                }
                return (true, "OK", dataModel);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool success, string message, List<SP_LeaveHistory> data)> GetAllLeaveHistory(string _siteID, string listID)
        {
            try
            {
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");
                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                    requestConfiguration.QueryParameters.Orderby = ["fields/Modified desc"];
                });

                List<SP_LeaveHistory> dataModel = new List<SP_LeaveHistory>();

                foreach (var record in recordList.Value)
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;
                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        decimal GetDecimal(string key) => decimal.TryParse(GetString(key), out var v) ? v : 0.0m;

                        DateTime? GetDate(string key)
                        {
                            if (data.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var dt))
                            {
                                return dt.ToLocalTime();
                            }
                            return null;
                        }
                        bool GetBool(string key) => bool.TryParse(GetString(key), out var v) ? v : false;

                        int GetEmpId()
                        {
                            if (data.TryGetValue("EmployeeID", out var val) && int.TryParse(val?.ToString(), out var id))
                            {
                                return id;
                            }
                            return 0;
                        }

                        string emailFromList = GetString("Email"); // assuming field_6 holds the email
                        if (!string.IsNullOrEmpty(emailFromList))
                        {
                            var item = new SP_LeaveHistory
                            {
                                EMP_NAME = GetString("Title"),
                                EMP_EMAIL = GetString("Email"),
                                EMP_NO = GetString("field_1"),
                                DATE_START = GetDate("field_2"),
                                DATE_END = GetDate("field_3"),
                                NUMBER_OF_LEAVE = GetDecimal("field_4"),
                                TYPE_LEAVE = GetString("field_5"),
                                APPROVER = GetString("field_6"),
                                APPROVER_DATE = GetDate("field_7"),
                                EMERGENCY_LEAVE = GetString("ContentType"),
                            };

                            dataModel.Add(item);
                        }

                    }

                }
                return (true, "OK", dataModel);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public async Task<(bool success, string message, SP_LeaveInformation data)> GetLeaveInformation(string _siteID, string listID, string email)
        {
            try
            {
                _ = _appClient ??
                   throw new NullReferenceException("Graph has not been initialized for app auth");
                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                    requestConfiguration.QueryParameters.Orderby = ["fields/Modified desc"];
                });

                SP_LeaveInformation dataModel = new SP_LeaveInformation();

                foreach (var record in recordList.Value)
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;
                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        decimal GetDecimal(string key) => decimal.TryParse(GetString(key), out var v) ? Math.Round(v, 1) : 0.0m;


                        string emailFromList = GetString("Email"); // assuming field_6 holds the email

                        if (!string.IsNullOrEmpty(emailFromList) && emailFromList.Equals(email, StringComparison.OrdinalIgnoreCase))
                        {
                            var item = new SP_LeaveInformation
                            {
                                Email = GetString("Email"),
                                Carry_Forward = GetDecimal("CarryForward"),
                                Annual_Leave = GetDecimal("field_3"),
                                Medical_Leave = GetDecimal("field_4"),
                                Paternity_Leave = GetDecimal("field_7"),
                                Maternity_Leave = GetDecimal("field_6"),
                                Compassionate_Leave = GetDecimal("field_9"),
                                Marriage_Leave = GetDecimal("field_8"),
                                Convocation_Leave = GetDecimal("ConvocationLeave"),
                                A_Carry_Forward = 7,
                                A_Annual_Leave = GetDecimal("AnnualLeaveEntitlement"),
                                A_Medical_Leave = GetDecimal("AnnualLeaveEntitlement"),
                                A_Paternity_Leave = 7,
                                A_Maternity_Leave = 98,
                                A_Compassionate_Leave = 3,
                                A_Marriage_Leave = 3,
                                A_Convocation_Leave = 3,
                            };

                            dataModel = item;
                        }
                    }
                }

                return (true, "OK", dataModel);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        #endregion

        #region"Public Holiday"
        public async Task<(bool success, string message, List<SP_PublicHoliday> data)> GetPublicHoliday(string _siteID, string listID)
        {
            try
            {
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");
                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                    requestConfiguration.QueryParameters.Orderby = ["fields/Modified desc"];
                });

                List<SP_PublicHoliday> dataModel = new List<SP_PublicHoliday>();

                foreach (var record in recordList.Value)
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;
                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        decimal GetDecimal(string key) => decimal.TryParse(GetString(key), out var v) ? v : 0.0m;

                        DateTime? GetDate(string key)
                        {
                            if (data.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var dt))
                            {
                                return dt.ToLocalTime();
                            }
                            return null;
                        }
                        bool GetBool(string key) => bool.TryParse(GetString(key), out var v) ? v : false;


                        var item = new SP_PublicHoliday
                        {
                            HOLIDAY_NAME = GetString("Title"),
                            DATE_START = GetDate("StartDate"),
                            DATE_END = GetDate("EndDate"),
                            NUMBER_OF_HOLIDAY = GetDecimal("Total_x0020_Days")
                        };

                        dataModel.Add(item);

                    }

                }
                return (true, "OK", dataModel);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        #endregion

        #region"Others"
        public async Task<(bool success, string message, List<SP_CompanySA> data)> GetCompanySA(string _siteID, string listID)
        {
            try
            {
                _ = _appClient ??
                    throw new NullReferenceException("Graph has not been initialized for app auth");
                var recordList = await _appClient.Sites[_siteID].Lists[listID].Items.GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Expand = ["fields"];
                    requestConfiguration.QueryParameters.Top = 10000;
                    requestConfiguration.QueryParameters.Orderby = ["fields/Modified desc"];
                });

                List<SP_CompanySA> dataModel = new List<SP_CompanySA>();

                foreach (var record in recordList.Value)
                {
                    if (record.Fields?.AdditionalData != null)
                    {
                        var data = record.Fields.AdditionalData;
                        // Helper Functions
                        string GetString(string key) => data.TryGetValue(key, out var val) ? val?.ToString() : null;
                        int GetInt(string key) => int.TryParse(GetString(key), out var v) ? v : 0;
                        double GetDouble(string key) => double.TryParse(GetString(key), out var v) ? v : 0.0;
                        decimal GetDecimal(string key) => decimal.TryParse(GetString(key), out var v) ? v : 0.0m;

                        DateTime? GetDate(string key)
                        {
                            if (data.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var dt))
                            {
                                return dt.ToLocalTime();
                            }
                            return null;
                        }
                        bool GetBool(string key) => bool.TryParse(GetString(key), out var v) ? v : false;


                        var item = new SP_CompanySA
                        {
                            SA_NAME = GetString("Title"),
                            SA_EMAIL = GetString("SAEmail")
                        };

                        dataModel.Add(item);

                    }

                }
                return (true, "OK", dataModel);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        #endregion

    }
}
