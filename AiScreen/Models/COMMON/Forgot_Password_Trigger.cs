using System.ComponentModel.DataAnnotations;

namespace SmartTemplateCore.Models.Common
{
    public class Forgot_Password_Trigger
    {
        [Required]
        public string Email { get; set; }
    }
}
