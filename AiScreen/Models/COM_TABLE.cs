using Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MaxSys.Enum.Enum;

namespace Component_TableListing.Models
{

    public class COM_TABLE : BaseStandardModel
    {
        public int COM_TABLE_ID { get; set; }
        public string NAME { get; set; }
        public string TABLE_NAME { get; set; }
        public string CLASS { get; set; }
        public string FIELD_ID_COLUMN { get; set; }
        public string VIEW_PATH { get; set; }
        public string AJAX_FETCHDATA_URL { get; set; }
        public string AJAX_FETCHDATA_DATA { get; set; }
        public Ajax_Type AJAX_FETCHDATA_TYPE { get; set; }
        public string AJAX_FETCHDATA_SUCCESS_URL { get; set; }
        public string AJAX_FETCHDATA_SUCCESS_FAILED { get; set; }
        public string AJAX_DELETEDATA_URL { get; set; }
        public string AJAX_DELETEDATA_DATA { get; set; }
        public Ajax_Type AJAX_DELETEDATA_TYPE { get; set; }
        public string AJAX_DELETEDATA_SUCCESS_URL { get; set; }
        public string AJAX_DELETEDATA_SUCCESS_FAILED { get; set; }
        public int LOAD_TYPE { get; set; }
        public string LOAD_STOR_PROC { get; set; }
        public string LOAD_STOR_PROC_PARAM { get; set; }
        public bool PANEL_STATUS_VISIBLE { get; set; }
        public string PANEL_STATUS_FIELD { get; set; }
        public string PANEL_STATUS_TITLE { get; set; }
        public bool AUDIT_TRAIL_VISIBLE { get; set; }
        public bool DELETE_RECORD_VISIBLE { get; set; }
        public bool VISIBLE_VIEW { get; set; }
        public string EXPORT_TITLE { get; set; }
        public string EXPORT_PDF { get; set; }
        public string EXPORT_PAPER { get; set; }
        public List<COM_TABLE_D> COM_TABLE_D { get; set; }
        public List<COM_STORE_PROC_PARAM> COM_STORE_PROC_PARAM { get; set; }
    }

}
