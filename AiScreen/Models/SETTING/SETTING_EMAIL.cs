namespace MaxSystemWebSite.Models.SETTING
{
    public class SETTING_EMAIL_DATA
    {
        public int DATACOUNT { get; set; }
    }
    public class SETTING_EMAIL
    {
        public int SETTING_EMAIL_ID { get; set; }
        public string DEFAULT_EMAIL { get; set; }
        public string FOLDER_NAME { get; set; }
        public string CLIENT_ID { get; set; }
        public string TENANT_ID { get; set; }
        public string CLIENT_SECRET { get; set; }
        public string GRAPH_USER { get; set; }
        public DateTime? EXPIRED_DATE_KEY { get; set; }
        public string INCIDENT_CONTROLLER_URL { get; set; }
        public string API_URL { get; set; }

        public int EMAIL_GRAB_COUNT { get; set; }
        public int? REC_TYPE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_LOC { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public string UPDATED_LOC { get; set; }
    }
}
