namespace MaxSys.Models
{
    public class Setting_Setup {
        public int ID { get; set; }
        public string CATEGORY { get; set; }
        public string SMTP_ACCOUNT { get; set; }
        public string SMTP_HOST { get; set; }
        public int SMTP_PORT { get; set; }
        public string WEB_URL { get; set; }
        public string APPLICATION_NAME { get; set; }
        public string HELP_DESK_EMAIL { get; set; }
    }
}
