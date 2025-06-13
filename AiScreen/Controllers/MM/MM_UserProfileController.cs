using BaseSQL.Interface;
using BaseWebApi.Interface;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.SETTING;
using MaxSystemWebSite.Models.USER;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MaxSystemWebSite.Controllers.MM
{
    public class MM_UserProfileController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<MM_UserProfileController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;
        private readonly IUserProfile _userProfile;

        public MM_UserProfileController(ILogger<MM_UserProfileController> logger, IConfiguration configuration, IWebApi webApi,
           IDapper dapper, IAuthenticator authenticator, UserProfileService userProfileService, ISharePoint sharePoint, IUserProfile userProfile)
       : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _userProfileService = userProfileService;
            _sharePoint = sharePoint;
            _userProfile = userProfile;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileDetail(string email)
        {
            (bool success, string message, SP_EmployeeInformation data) dataReturn = await _sharePoint.GetEmployeeInformationDetail(_configuration["MaxSystem_SharePoint:MainPoint"], _configuration["MaxSystem_SharePoint:EmployeeInformation"], email);

            return Json(new { success = dataReturn.success, msg = dataReturn.message, data = dataReturn.data });
        }

        [HttpPost]
        public async Task<IActionResult> GetUserList() 
        {

           (bool success, string message, List<USER_PROFILE> ListData) dataReturn = await _userProfile.GetUserList();

            return Json(new { success = dataReturn.success, msg = dataReturn.message, data = dataReturn.ListData });
        }
    }
}
