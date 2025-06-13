using Base.Model;

namespace MaxSys.Models.MM
{
    public class EMPLOYEE : BaseStandardModel
    {
        public string USER_ID { get; set; }
        public int EMPLOYEE_ID { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string WORK_NO { get; set; }
        public string POSITION { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }


        public DateTime? DATE_CONFIRMED { get; set; }

        private DateTime? _DATE_JOINED = DateTime.Now;

        public DateTime? DATE_JOINED
        {
            get { return _DATE_JOINED; }
            set
            {
                _DATE_JOINED = value;
            }
        }
        public string SUPERVISOR_NAME { get; set; }
        public string SUPERVISOR_EMAIL { get; set; }
        public string SA_NAME { get; set; }
        public string SA_EMAIL { get; set; }
        public int ACCESS_LEVEL { get; set; }
        public string ACCESS_LEVEL_DESC { get; set; }
        public List<Setting_DropDown> ddlDepartment { get; set; }
        public List<Setting_DropDown> ddlDivision { get; set; }
        public List<Setting_DropDown> ddlAccessLevel { get; set; }
    }
}
