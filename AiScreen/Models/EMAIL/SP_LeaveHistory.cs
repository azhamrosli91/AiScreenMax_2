namespace MaxSystemWebSite.Models.EMAIL
{
    public class SP_LeaveHistory
    {
        public SP_LeaveHistory()
        {
        }

        public SP_LeaveHistory(string eMP_NAME, string eMP_EMAIL, string eMP_NO, DateTime? dATE_START, DateTime? dATE_END, decimal nUMBER_OF_LEAVE, string tYPE_LEAVE, string aPPROVER, DateTime? aPPROVER_DATE, string eMERGENCY_LEAVE)
        {
            EMP_NAME = eMP_NAME;
            EMP_EMAIL = eMP_EMAIL;
            EMP_NO = eMP_NO;
            DATE_START = dATE_START;
            DATE_END = dATE_END;
            NUMBER_OF_LEAVE = nUMBER_OF_LEAVE;
            TYPE_LEAVE = tYPE_LEAVE;
            APPROVER = aPPROVER;
            APPROVER_DATE = aPPROVER_DATE;
            EMERGENCY_LEAVE = eMERGENCY_LEAVE;
        }

        public string EMP_NAME { get; set; }
        public string EMP_EMAIL { get; set; }
        public string EMP_NO { get; set; }
        public DateTime? DATE_START { get; set; }
        public DateTime? DATE_END { get; set; }
        public decimal NUMBER_OF_LEAVE { get; set; }
        public string TYPE_LEAVE { get; set; }
        public string APPROVER { get; set; }
        public DateTime? APPROVER_DATE { get; set; }
        public string EMERGENCY_LEAVE { get; set; }
        private string _DATE_START_DESC;

        public string DATE_START_DESC
        {
            get { return DATE_START?.ToString("dd-MMM-yyyy"); }
            set { _DATE_START_DESC = value; }
        }

        private string _DATE_END_DESC;

        public string DATE_END_DESC
        {
            get { return DATE_END?.ToString("dd-MMM-yyyy"); }
            set { _DATE_END_DESC = value; }
        }
    }
}
