using System.ComponentModel.DataAnnotations;

namespace SmartTemplateCore.Models.Common
{
    public class Forgot_Password_OTP_Verify
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string OTP { get; set; }
    }
}
