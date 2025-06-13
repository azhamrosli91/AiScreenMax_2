using Base.Model;

namespace MaxSys.Models.MM
{
    public class MM_USER : BaseStandardModel
    {
        public int ID_MM_USER { get; set; }
        public string NAME { get; set; }
        public string EMAIL { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE_CODE_1 { get; set; }
        public string PHONE_NO_1 { get; set; }
        public string PHONE_CODE_2 { get; set; }
        public string PHONE_NO_2 { get; set; }
        public bool? GENDER { get; set; }
        public DateTime? DATE_OF_BIRTH { get; set; }
        public DateTime? DATETIME { get; set; }
        public string DATETIME_DESC { get; set; }
        public string PHONE_FULL { get; set; }
        public string PHONE_2_FULL { get; set; }
        public string ACTION { get; set; }
        public string ROLE_DESC { get; set; }
        public int ROLE { get; set; }
        public int USER_ROLE { get; set; }
        public string PROFILE_IMAGE { get; set; }
        public string PROFILE_IMAGE_TMP { get; set; }
        public List<Setting_DropDown> ddlROLE { get; set; }
        public List<Setting_DropDown> ddlSTATE { get; set; }
        public List<Setting_DropDown> ddlCOUNTRY { get; set; }
        public List<Setting_DropDown> ddlPHONE_CODE { get; set; }

    }
}
