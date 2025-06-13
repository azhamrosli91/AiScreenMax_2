using System.ComponentModel.DataAnnotations;

namespace SmartTemplateCore.Models.Common
{
    public class Forgot_Password
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string OTP { get; set; }
        [Required]
        public string NewPass { get; set; }
        [Required]
        public string ConfirmNewPass { get; set; }
    }
}
