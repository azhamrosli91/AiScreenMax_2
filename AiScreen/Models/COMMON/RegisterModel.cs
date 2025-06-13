using Base.Model;

namespace SmartTemplateCore.Models.Common
{
    public class RegisterModel : RegisterViewModel
    {
        public string COMPANY_NAME { get; set; }
        public List<Setting_DropDown> ddlCOMPANY { get; set; }
    }
}
