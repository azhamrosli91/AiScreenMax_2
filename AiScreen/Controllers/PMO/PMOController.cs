using BaseSQL.Interface;
using BaseWebApi.Interface;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSystemWebSite.Controllers.DE;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MaxSystemWebSite.Controllers.PMO
{
    public class PMOController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<PMOController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;
        private readonly IHttpClientFactory _clientFactory;

        public PMOController(ILogger<PMOController> logger, IConfiguration configuration, IWebApi webApi,
           IDapper dapper, IAuthenticator authenticator, UserProfileService userProfileService, ISharePoint sharePoint, IHttpClientFactory clientFactory)
            : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _userProfileService = userProfileService;
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TaskUpdate()
        {
            return View();
        }
    }
}
