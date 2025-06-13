using Component_TableListing.Models;

namespace BaseModel.Models.Component
{
    public class ComponentBottomPanelModel
    {
        #region "Delete Button"
        public string ButtonID_Delete { get; set; } = "btnDelete";
        public string ButtonName_Delete { get; set; }
        public string ButtonText_Delete { get; set; } = "Delete";
        public string ButtonColor_Delete { get; set; }
        public string ButtonTextColor_Delete { get; set; }
        public string ButtonType_Delete { get; set; } = "button";
        public string ButtonPath_Delete { get; set; }
        public bool ButtonVisible_Delete { get; set; } = false;
        public string ButtonDisabled_Delete { get; set; } = "";
        public string ButtonTitle_Delete { get; set; } = "";
        public string ButtonIcon_Delete { get; set; } = "fa-trash";
        public string ButtonIcon_Color_Delete { get; set; } = "";
        public ComponentTable_AJAXModel AjaxInfo_FetchData_Delete { get; set; }
        #endregion

        #region "Export Button"
        public string ButtonID_Export { get; set; } = "btnExport";
        public string ButtonName_Export { get; set; }
        public string ButtonText_Export { get; set; } = "Export";
        public string ButtonColor_Export { get; set; }
        public string ButtonTextColor_Export { get; set; }
        public string ButtonType_Export { get; set; } = "button";
        public string ButtonPath_Export { get; set; }
        public string ButtonFunctionName_Export { get; set; }
        public bool ButtonVisible_Export { get; set; } = false;
        public string ButtonIcon_Export { get; set; } = "";
        public string ButtonIcon_Color_Export { get; set; } = "";
        #endregion

        #region "Back Button"
        public string ButtonID_Back { get; set; } = "btnBack";
        public string ButtonName_Back { get; set; }
        public string ButtonText_Back { get; set; } = "Back";
        public string ButtonColor_Back { get; set; }
        public string ButtonTextColor_Back { get; set; }
        public string ButtonType_Back { get; set; } = "button";
        public string ButtonPath_Back { get; set; }
        public string ButtonFunctionName_Back { get; set; }
        public bool ButtonVisible_Back { get; set; } = false;
        public string ButtonIcon_Back { get; set; } = "";
        public string ButtonIcon_Color_Back { get; set; } = "";
        public string ButtonURL_Back { get; set; }
        #endregion
        #region "Save As Draft Button"
        public string ButtonID_SaveAsDraft { get; set; } = "btnSaveAsDraft";
        public string ButtonName_SaveAsDraft { get; set; }
        public string ButtonText_SaveAsDraft { get; set; } = "Save As Draft";
        public string ButtonColor_SaveAsDraft { get; set; }
        public string ButtonTextColor_SaveAsDraft { get; set; }
        public string ButtonType_SaveAsDraft { get; set; } = "button";
        public string ButtonPath_SaveAsDraft { get; set; }
        public string ButtonFunctionName_SaveAsDraft { get; set; }
        public bool ButtonVisible_SaveAsDraft { get; set; } = false;
        public string ButtonDisabled_SaveAsDraft { get; set; } = "";
        public string ButtonTitle_SaveAsDraft { get; set; } = "";
        public string ButtonIcon_SaveAsDraft { get; set; } = "";
        public string ButtonIcon_Color_SaveAsDraft { get; set; } = "";
        #endregion
        #region "Submit Button"
        public string ButtonID_Submit { get; set; } = "btnSubmit";
        public string ButtonName_Submit { get; set; }
        public string ButtonText_Submit { get; set; } = "Submit";
        public string ButtonColor_Submit { get; set; }
        public string ButtonTextColor_Submit { get; set; }
        public string ButtonType_Submit { get; set; } = "button";
        public string ButtonPath_Submit { get; set; }
        public string ButtonFunctionName_Submit { get; set; }
        public bool ButtonVisible_Submit { get; set; } = false;
        public string ButtonDisabled_Submit { get; set; } = "";
        public string ButtonTitle_Submit { get; set; } = "";
        public string ButtonIcon_Submit { get; set; } = "";
        public string ButtonIcon_Color_Submit { get; set; } = "";
        public bool ButtonVisible_SubmitUsingDefault { get; set; } = false;
        public ComponentTable_AJAXModel AjaxInfo_FetchData_Submit { get; set; }
        #endregion

    }
}
