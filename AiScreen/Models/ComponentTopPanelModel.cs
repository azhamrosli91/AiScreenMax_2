using Component_TableListing.Models;
using MaxSys.Interface;

namespace MaxSys.Models
{
    public class ComponentTopPanelModel : IButton, ILabel
    {
        #region "Delete Button"
        public string Del_ButtonID_Delete { get; set; } = "btnDelete";
        public string Del_ButtonName_Delete { get; set; }
        public string Del_ButtonText_Delete { get; set; } = "Delete";
        public string Del_ButtonColor_Delete { get; set; }
        public string Del_ButtonTextColor_Delete { get; set; }
        public string Del_ButtonType_Delete { get; set; } = "button";
        public string Del_ButtonPath_Delete { get; set; }
        public bool Del_ButtonVisible_Delete { get; set; } = false;
        public string Del_ButtonDisabled_Delete { get; set; } = "";
        public string Del_ButtonTitle_Delete { get; set; } = "";
        public string Del_ButtonIcon_Delete { get; set; } = "fa-trash";
        public string Del_ButtonIcon_Color_Delete { get; set; } = "";
        public ComponentTable_AJAXModel Del_AjaxInfo_FetchData_Delete { get; set; }
        #endregion
        #region "Back Button"
        public string Back_ButtonID_Back { get; set; } = "btnBack";
        public string Back_ButtonName_Back { get; set; }
        public string Back_ButtonText_Back { get; set; } = "Back";
        public string Back_ButtonColor_Back { get; set; }
        public string Back_ButtonTextColor_Back { get; set; }
        public string Back_ButtonType_Back { get; set; } = "button";
        public string Back_ButtonPath_Back { get; set; }
        public string Back_ButtonFunctionName_Back { get; set; }
        public bool Back_ButtonVisible_Back { get; set; } = false;
        public string Back_ButtonIcon_Back { get; set; } = "";
        public string Back_ButtonIcon_Color_Back { get; set; } = "";
        public string Back_ButtonURL_Back { get; set; }
        #endregion
        #region "Save As Draft Button"
        public string SaveAsDraft_ButtonID_SaveAsDraft { get; set; } = "btnSaveAsDraft";
        public string SaveAsDraft_ButtonName_SaveAsDraft { get; set; }
        public string SaveAsDraft_ButtonText_SaveAsDraft { get; set; } = "Save As Draft";
        public string SaveAsDraft_ButtonColor_SaveAsDraft { get; set; }
        public string SaveAsDraft_ButtonTextColor_SaveAsDraft { get; set; }
        public string SaveAsDraft_ButtonType_SaveAsDraft { get; set; } = "button";
        public string SaveAsDraft_ButtonPath_SaveAsDraft { get; set; }
        public string SaveAsDraft_ButtonFunctionName_SaveAsDraft { get; set; }
        public bool SaveAsDraft_ButtonVisible_SaveAsDraft { get; set; } = false;
        public string SaveAsDraft_ButtonDisabled_SaveAsDraft { get; set; } = "";
        public string SaveAsDraft_ButtonTitle_SaveAsDraft { get; set; } = "";
        public string SaveAsDraft_ButtonIcon_SaveAsDraft { get; set; } = "";
        public string SaveAsDraft_ButtonIcon_Color_SaveAsDraft { get; set; } = "";
        #endregion
        #region "Submit Button"
        public string Submit_ButtonID_Submit { get; set; } = "btnSubmit";
        public string Submit_ButtonName_Submit { get; set; }
        public string Submit_ButtonText_Submit { get; set; } = "Submit";
        public string Submit_ButtonColor_Submit { get; set; }
        public string Submit_ButtonTextColor_Submit { get; set; }
        public string Submit_ButtonType_Submit { get; set; } = "button";
        public string Submit_ButtonPath_Submit { get; set; }
        public string Submit_ButtonFunctionName_Submit { get; set; }
        public bool Submit_ButtonVisible_Submit { get; set; } = false;
        public string Submit_ButtonDisabled_Submit { get; set; } = "";
        public string Submit_ButtonTitle_Submit { get; set; } = "";
        public string Submit_ButtonIcon_Submit { get; set; } = "";
        public string Submit_ButtonIcon_Color_Submit { get; set; } = "";
        public bool Submit_ButtonVisible_SubmitUsingDefault { get; set; } = false;
        public ComponentTable_AJAXModel Submit_AjaxInfo_FetchData_Submit { get; set; }
        #endregion
        #region "Title Header"
        public string Title_LabelID { get; set; } = "lblHeaderTitle";
        public string Title_LabelName { get; set; } = "lblHeader";
        public string Title_LabelText { get; set; } = "Main Listing";
        public string Title_LabelColor { get; set; } = "";
        public bool Title_LabelVisible { get; set; } = true;
        #endregion
        #region "Create Form"
        public string CreateForm_ButtonID { get; set; } = "btnCreateForm";
        public string CreateForm_ButtonName { get; set; }
        public string CreateForm_ButtonText { get; set; } = "Create New Form";
        public string CreateForm_ButtonColor { get; set; }
        public string CreateForm_ButtonTextColor { get; set; }
        public string CreateForm_ButtonType { get; set; }
        public string CreateForm_ButtonPath { get; set; }
        public string CreateForm_ButtonFunctionName { get; set; }
        public bool CreateForm_ButtonVisible { get; set; } = false;
        public string CreateForm_ButtonDisabled { get; set; } = "";
        public string CreateForm_ButtonTitle { get; set; } = "";
        public string CreateForm_ButtonIcon { get; set; } = "fa-plus";
        public string CreateForm_ButtonIcon_Color { get; set; } = "text-primary";
        #endregion
        #region "Edit"
        public string Edit_ButtonEditID { get; set; } = "btnEdit";
        public string Edit_ButtonEditName { get; set; }
        public string Edit_ButtonEditText { get; set; } = "Edit";
        public string Edit_ButtonEditColor { get; set; }
        public string Edit_ButtonEditTextColor { get; set; }
        public string Edit_ButtonEditType { get; set; }
        public string Edit_ButtonEditPath { get; set; }
        public string Edit_ButtonEditFunctionName { get; set; }
        public bool Edit_ButtonEditVisible { get; set; } = false;
        public string Edit_ButtonEditDisabled { get; set; } = "";
        public string Edit_ButtonEditTitle { get; set; } = "";
        public string Edit_ButtonEditIcon { get; set; } = "";
        public string Edit_ButtonEditIcon_Color { get; set; } = "text-primary";
        #endregion
        #region "Export"
        public string Export_ButtonExport_ID { get; set; } = "btnExport";
        public string Export_ButtonExport_Text { get; set; } = "Export";
        public string Export_ButtonExport_Color { get; set; }
        public string Export_ButtonExport_TextColor { get; set; }
        public bool Export_ButtonExport_Visible { get; set; } = false;
        public string Export_ButtonExport_PDF_ID { get; set; } = "btnExportPDF";
        public string Export_ButtonExport_PDF_Name { get; set; }
        public string Export_ButtonExport_PDF_Text { get; set; } = "Export PDF";
        public string Export_ButtonExport_PDF_Color { get; set; }
        public string Export_ButtonExport_PDF_TextColor { get; set; }
        public string Export_ButtonExport_PDF_Type { get; set; }
        public string Export_ButtonExport_PDF_Path { get; set; }
        public string Export_ButtonExport_PDF_FunctionName { get; set; }
        public bool Export_ButtonExport_PDF_Visible { get; set; } = false;
        public string Export_ButtonExport_PDF_Icon { get; set; } = "fa-file";
        public string Export_ButtonExport_PDF_Icon_Color { get; set; } = "text-primary";

        public string Export_ButtonExport_XLS_ID { get; set; } = "btnExportXLS";
        public string Export_ButtonExport_XLS_Name { get; set; }
        public string Export_ButtonExport_XLS_Text { get; set; } = "Export Excel";
        public string Export_ButtonExport_XLS_Color { get; set; }
        public string Export_ButtonExport_XLS_TextColor { get; set; }
        public string Export_ButtonExport_XLS_Type { get; set; }
        public string Export_ButtonExport_XLS_Path { get; set; }
        public string Export_ButtonExport_XLS_FunctionName { get; set; }
        public bool Export_ButtonExport_XLS_Visible { get; set; } = false;
        public string Export_ButtonExport_XLS_Icon { get; set; } = "fa-table";
        public string Export_ButtonExport_XLS_Icon_Color { get; set; } = "text-primary";

        #endregion
        #region "Import"
        public string Import_ButtonImportID { get; set; } = "btnImport";
        public string Import_ButtonImportName { get; set; }
        public string Import_ButtonImportText { get; set; } = "Import";
        public string Import_ButtonImportColor { get; set; }
        public string Import_ButtonImportTextColor { get; set; }
        public string Import_ButtonImportType { get; set; }
        public string Import_ButtonImportPath { get; set; }
        public string Import_ButtonImportFunctionName { get; set; }
        public bool Import_ButtonImportVisible { get; set; } = false;
        public string Import_ButtonImportIcon { get; set; } = "fa-cloud-arrow-up";
        public string Import_ButtonImportIcon_Color { get; set; } = "text-primary";
        #endregion
    }
}
