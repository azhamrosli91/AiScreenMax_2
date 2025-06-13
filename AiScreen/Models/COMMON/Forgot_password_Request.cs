namespace SmartTemplateCore.Models.Common
{
    public class Forgot_password_Request
    {
        public string EmailAddress { get; set; }
        public string CompanyCode { get; set; }
        public string ResetPasswordURL { get; set; }
        public string DatabaseType { get; set; }
        public string SystemName { get; set; }

    }
}
