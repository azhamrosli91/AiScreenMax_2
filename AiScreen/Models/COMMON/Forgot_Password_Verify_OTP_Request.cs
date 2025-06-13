namespace SmartTemplateCore.Models.Common
{
    public class Forgot_Password_Verify_OTP_Request
    {
        public string EmailAddress { get; set; }
        public string OTP { get; set; }
        public string CompanyCode { get; set; }
        public string DatabaseType { get; set; }
    }
}
