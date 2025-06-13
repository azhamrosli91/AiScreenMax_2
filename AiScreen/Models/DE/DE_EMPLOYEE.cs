using Base.Model;

namespace MaxSys.Models.DE
{
    public class DE_EMPLOYEE : BaseStandardModel
    {
        public int ID_DE_EMPLOYEE { get; set; }
        public int ID_MM_COMPANY { get; set; }
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string EMAIL { get; set; }
        public string EMAIL_PERSONAL { get; set; }
        public DateTime? DATE_JOIN { get; set; }
        public string DESIGNATION { get; set; }
        public string DIVISION { get; set; }
        public string DEPARTMENT { get; set; }
        public DateTime? RESIGN_DATE { get; set; }
        public DateTime? LAST_WORK_DATE { get; set; }
        public DateTime? DATE_OF_BIRTH { get; set; }
        public int? GENDER { get; set; }
        public string RACE { get; set; }
        public string MARITAL { get; set; }
        public string HIGH_EDUCATION { get; set; }
        public string ADDRESS_1 { get; set; }
        public string ADDRESS_2 { get; set; }
        public string ADDRESS_3 { get; set; }
        public string POSTCODE { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string COUNTRY { get; set; }
        public string IC_NO { get; set; }
        public string PHONE_1_CODE { get; set; }
        public string PHONE_1 { get; set; }
        public string PHONE_2_CODE { get; set; }
        public string PHONE_2 { get; set; }
        public string PHOTO_URL { get; set; }
        public string PHOTO_URL_TMP { get; set; }
        public List<Setting_DropDown> ddlSTATE { get; set; }
        public List<Setting_DropDown> ddlCOUNTRY { get; set; }
        public List<Setting_DropDown> ddlSTATUS { get; set; }
        public List<Setting_DropDown> ddlPHONE_CODE { get; set; }
        public List<Setting_DropDown> ddlRACE { get; set; }
        public List<Setting_DropDown> ddlRELIGION { get; set; }
        public List<Setting_DropDown> ddlGENDER { get; set; }
    }

}
