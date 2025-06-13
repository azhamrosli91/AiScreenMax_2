using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static MaxSys.Enum.Enum;

namespace Component_TableListing.Models
{
    public class ComponentTableModel
    {
        public string TableID { get; set; } = "pv_inclist_datatable";
        public string TableName { get; set; } = "tblListing";
        public string Class { get; set; }
        public int LoadType { get; set; } = 0;
        public bool TableVisible { get; set; } = true;
        public string Field_ID { get; set; }
        public string ViewPath { get; set; }
        public bool PanelStatusVisible { get; set; } = true;
        public string PanelStatusField { get; set; }
        public string Delete_Right { get; set; } = "";
        public string View_Right { get; set; } = "";
        public string Disabled { get; set; } = "";
        public string PanelStatusTitle { get; set; }
        public bool AuditTrailVisible { get; set; }= true;
        public bool DeleteRecordVisible { get; set; } = true;
        public bool Visible_View { get; set; } = true;
        public string GroupByColumn { get; set; } = "";
        public string Export_pdf_rotation { get; set; } = "portrait";
        public string Export_pdf_Paper_Size { get; set; } = "A4";
        public string Export_pdf_Title { get; set; } = "";

        public ComponentTable_AJAXModel AjaxInfo_FetchData { get; set; }
        public ComponentTable_AJAXModel AjaxInfo_DeleteData { get; set; }
        public string FetchData_Param { get; set; }
        public List<ComponentTable_ColumnModel> listComponentTable_ColumnModels { get; set; }
        public int DataModelCount { get; set; }
        private List<dynamic> _DataModel;

        public List<dynamic> DataModel
        {
            get { return _DataModel; }
            set { 
                    _DataModel = value;
                //2 > 0 ? "1" : "0"
                DataModelCount = (value.Count > 0) ? 0 : 1;
            }
        }


    }
    public class ComponentTable_AJAXModel 
    {
        public ComponentTable_AJAXModel() { }
        public ComponentTable_AJAXModel(string uRL_Ajax, Ajax_Type type_Ajax, string? data_Ajax = null,string success_URL = null, string error_URL = null)
        {
            URL_Ajax = uRL_Ajax;
            Type_Ajax = type_Ajax;
            Data_Ajax = data_Ajax;
            Success_URL = success_URL;
            Error_URL = error_URL;
        }

        public string URL_Ajax { get; set; }
        public Ajax_Type Type_Ajax { get; set; } = Ajax_Type.GET; //GET OR POST OR PUT OR DELETE
        public string? Data_Ajax { get; set; }
        public string? Success_URL { get; set; }
        public string? Error_URL { get; set; }
    }

    public class ComponentTable_ColumnModel 
    {
        public ComponentTable_ColumnModel(int index, string columnName, string columnField, string style, 
            string @class, string width,string dataFieldType = "text",bool isallowFilter = true, bool isvisible = true, string customComponent = "", bool isexport = true)
        {
            Index = index;
            ColumnName = columnName;
            ColumnField = columnField;
            Style = style;
            Class = @class;
            Width = width;
            isAllowFilter = isallowFilter;
            isVisible = isvisible;
            DataFieldType = dataFieldType;
            CustomComponent = customComponent;
            isExport = isexport;
        }

        public int Index { get; set; }
        public string DataFieldType { get; set; }
        public string ColumnName { get; set; }
        public string ColumnField { get; set; }
        public string Style { get; set; }
        public string Class { get; set; }
        public string Width { get; set; }
        public bool isCustomComponent { get; set; }
        public bool isAllowFilter { get; set; }
        public bool isVisible { get; set; }
        public bool isExport { get; set; } = true;

        private string customComponent;

        public string CustomComponent
        {
            get { return customComponent; }
            set { 
                   customComponent = WebUtility.HtmlEncode(value);
                   if (!string.IsNullOrEmpty(value)) {
                       isCustomComponent = true;
                   }
                   else {
                       isCustomComponent = false;
                   }
                }
        }

    }
}
