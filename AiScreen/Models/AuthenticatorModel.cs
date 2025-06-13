using System.ComponentModel.DataAnnotations;

namespace MaxSys.Models
{
    public class AuthenticatorModel
    {
        public int ACL_USER_ID { get; set; }
        public string USER_ID { get; set; }
        public string USR_EMAIL { get; set; }
        public string COMPANY { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public int ACL_ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public int ACL_RESOURCE_ID { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_DESC { get; set; }
        public bool VALID_USER { get; set; }

        [Required(ErrorMessage = "* Please Enter Username.")]
        [Display(Name = "Username")]
        public string LOGIN_ID { get; set; }

        [Required(ErrorMessage = "* Please Enter password.")]
        [Display(Name = "Password")]
        public string PASSWORD { get; set; }

        public string PASSWORD_HASH { get; set; }

        public string RETURN_URL { get; set; }
    }
}
