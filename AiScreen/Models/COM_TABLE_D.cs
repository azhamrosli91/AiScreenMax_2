using Base.Model;

namespace Component_TableListing.Models
{
    public class COM_TABLE_D : BaseStandardModel
    {
        public int COM_TABLE_D_ID { get; set; }
        public int COM_TABLE_ID { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_TITLE { get; set; }
        public string COLUMN_DATATYPE { get; set; }
        public string INPUT_TYPE { get; set; } = "text";
        public string COLUMN_LENGTH { get; set; }
        public int? SEQ { get; set; }
        public int? WIDTH_PX { get; set; }
        public string STYLE { get; set; }
        public string CLASS { get; set; }
        public bool? IS_CUSTOM_COMPONENT { get; set; }
        public string CUSTOM_COMPONENT { get; set; }
        public bool? VISIBLE { get; set; }
        public bool? ALLOW_FILTER { get; set; }
        public bool? ALLOW_EXPORT { get; set; }

    }
}
