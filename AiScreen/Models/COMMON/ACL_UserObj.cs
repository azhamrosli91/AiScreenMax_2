namespace Base.Model
{
    public class ACL_UserObj
    {
        public string JWT_TOKEN { get; set; }
        public string JWT_REFRESH_TOKEN { get; set; }
        public int MM_EMPLOYEE_ID { get; set; }
        public int ACL_USER_ID { get; set; }
        public int ACL_ROLE_ID { get; set; }
        public int ACL_RESOURCE_ID { get; set; }
        public string USER_ID { get; set; }
        public string USR_EMAIL { get; set; }

        private string COMPANY_;

        public string COMPANY
        {
            get { return COMPANY_; }
            set
            {
                COMPANY_ = (value == null || value == "") ? "TMS" : value;
                COMPANY_CODE = (value == null || value == "") ? "TMS" : value;
            }
        }

        public string EMP_NO { get; set; }
        //public string EMP_NAME { get; set; }
        private string EMP_NAME_;

        public string EMP_NAME
        {
            get { return EMP_NAME_.ToString().ToUpper(); }
            set { EMP_NAME_ = value; }
        }

        public string ROLE_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_DESC { get; set; }
        public string TITLE_MODULE { get; set; }

        public string SECTION { get; set; } = "";
        public int MM_DEPARTMENT_ID { get; set; } = 0;
        public int ID_COMPANY { get; set; } = 0;
        public string COMPANY_CODE { get; set; }
        public string IP_ADDRESS { get; set; }
    }

}
