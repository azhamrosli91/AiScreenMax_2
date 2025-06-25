namespace MaxSystemWebSite.Models.DE
{
    public class DE_SUBCRIPTION_TRANS 
    {
        public Guid DE_SUBCRIPTION_TRANS_ID { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string BILL_CODE { get; set; }
        public string BILL_NAME { get; set; }
        public string BILL_DESCRIPTION { get; set; }
        public int? BILL_PRICE_SETTING { get; set; }
        public int? BILL_PAYOR_INFO { get; set; }
        public decimal? BILL_AMOUNT { get; set; }
        public string BILL_RETURN_URL { get; set; }
        public string BILL_CALLBACK_URL { get; set; }
        public string BILL_EXTERNAL_REFNO { get; set; }
        public string BILL_TO { get; set; }
        public string BILL_EMAIL { get; set; }
        public string BILL_PHONE { get; set; }
        public int? STATUS { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
    }
}



