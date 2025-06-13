using BaseSQL.Interface;
using BaseWebApi.Interface;
using Component_TableListing.Models;
using MaxSys.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using MaxSys.Controllers;
using MaxSys.Helpers;
using MaxSys.Interface;

namespace MaxSys.Controllers.DE
{
    public class SearchCandidateController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<SearchCandidateController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        public SearchCandidateController(ILogger<SearchCandidateController> logger, IConfiguration configuration, IWebApi webApi,
            IDapper dapper, IJWTToken jWTToken, ISQL sql,
            IDapper_Oracle dapper_Oracle, HtmlEncoder htmlEncoder, IAuthenticator authenticator, IEmailService emailService, IWebHostEnvironment environment)
        : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _jwtToken = jWTToken;
            _SQL = sql;
            _htmlEncoder = htmlEncoder;
            _dapper_Oracle = dapper_Oracle;
            _emailService = emailService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult SearchCandidates(string search)
        {
            var candidates = new List<Candidate>
    {
        new Candidate { Id = 1, Name = "John Doe", Skills = "C#, ASP.NET, SQL" },
        new Candidate { Id = 2, Name = "Jane Smith", Skills = "C#, JavaScript, React, Node.js" },
        new Candidate { Id = 3, Name = "Michael Johnson", Skills = "C#, Python, Machine Learning" }
    };

            var results = candidates
                .Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                         || c.Skills.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Json(results);
        }
    }
}
