using MaxSystemWebSite.Models.EMAIL;

namespace MaxSystemWebSite.Models.MM
{
    public class MM_LEAVE_VM
    {
        public string EMP_NAME { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_MANAGER { get; set; }
        public List<SP_CompanySA> DDL_MANAGER { get; set; }
    }

    public class MM_LEAVE_APPLICATION
    {
        public string EMP_NAME { get; set; }
        public string EMP_NO { get; set; }
        public string MANAGER_NAME { get; set; }
        public string MANAGER_EMAIL { get; set; }
        public string LEAVE_TYPE { get; set; }
        public bool IS_HALF_DAY { get; set; }
        public DateTime DATE_FROM { get; set; }
        public DateTime DATE_TO { get; set; }
        public string DAYS_APPLIED { get; set; }
        public string JUSTIFICATION { get; set; }
        public string EMAIL_CC { get; set; }
        public List<MM_LEAVE_ATTACHMENT> ListMM_LEAVE_ATTACHMENT { get; set; }
    }

    public class MM_LEAVE_ATTACHMENT
    {
        public string ATTACHMENT_NAME { get; set; }
        public string ATTACHMENT_URL { get; set; }
        public IFormFile ATTACHMENT_FILEBASE { get; set; }
    }

}
