using Base.Model;

namespace SmartTemplateCore.Models.Common
{
    public class User_login_System : BaseStandardModel
    {
        public int USER_LOGIN_ID { get; set; }
        public int MM_EMPLOYEE_ID { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; } 
    }
}
