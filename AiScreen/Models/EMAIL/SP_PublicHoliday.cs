namespace MaxSystemWebSite.Models.EMAIL
{
    public class SP_PublicHoliday
    {
        public SP_PublicHoliday()
        {
        }

        public SP_PublicHoliday(string hOLIDAY_NAME, DateTime? dATE_START, DateTime? dATE_END, decimal nUMBER_OF_HOLIDAY)
        {
            HOLIDAY_NAME = hOLIDAY_NAME;
            DATE_START = dATE_START;
            DATE_END = dATE_END;
            NUMBER_OF_HOLIDAY = nUMBER_OF_HOLIDAY;
        }

        public string HOLIDAY_NAME { get; set; }
        public DateTime? DATE_START { get; set; }
        public DateTime? DATE_END { get; set; }
        
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

        public decimal NUMBER_OF_HOLIDAY { get; set; }

    }
}