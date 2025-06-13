using Base.Model;
using BaseSQL.Interface;
using MaxSys.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MaxSys.Controllers.Components
{
    public class ComponentTopPanelViewComponent : ViewComponent
    {
        private readonly IDapper _dapper;
        public ComponentTopPanelViewComponent(IDapper dapper)
        {
            _dapper = dapper;
        }
        public virtual async Task<IViewComponentResult> InvokeAsync(ComponentTopPanelModel componentTopPanel)
        {
            if (componentTopPanel == null) { componentTopPanel = new ComponentTopPanelModel(); }

            // Accessing cookies from the HttpContext
            var USER_ID = HttpContext.Request.Cookies["USER_ID"];
            var ID_MM_COMPANY = HttpContext.Request.Cookies["ID_MM_COMPANY"] ?? "1";
            var ACCESS_LEVEL = HttpContext.Request.Cookies["ACCESS_LEVEL"] ?? "1";
            // Access the current controller and action names
            var controllerName = HttpContext.Request.RouteValues["controller"]?.ToString();
            var actionName = HttpContext.Request.RouteValues["action"]?.ToString();

            (bool success, string message, List<AclResource> item) side_bar = await _dapper.PSP_COMMON_DAPPER<AclResource>("PSP_ACL_RESOURCE_CONTROL", 
                CommandType.StoredProcedure, new { USER_ID, ID_MM_COMPANY , RESOURCE_CONTROLLER =controllerName });

            if (ICommonMethod.isNumeric(ACCESS_LEVEL.ToString()) == false) {
                ACCESS_LEVEL = "1";
            }
            if (side_bar.success == true && side_bar.item != null)
            {
                bool btnAdd = true;

                foreach (AclResource item in side_bar.item)
                {
                    if (item.ADD_RIGHT <= Convert.ToInt32(ACCESS_LEVEL))
                    {
                        componentTopPanel.CreateForm_ButtonDisabled = "";
                        componentTopPanel.CreateForm_ButtonTitle = string.IsNullOrEmpty(componentTopPanel.CreateForm_ButtonTitle)
                                                          ? componentTopPanel.CreateForm_ButtonText
                                                          : componentTopPanel.CreateForm_ButtonTitle;
                    }
                    else {
                        componentTopPanel.CreateForm_ButtonDisabled = "disabled";
                        componentTopPanel.CreateForm_ButtonTitle = "Anda tidak mempunyai akses";
                    }

                    if (item.EDIT_RIGHT <= Convert.ToInt32(ACCESS_LEVEL))
                    {
                        componentTopPanel.Edit_ButtonEditDisabled = "";
                        componentTopPanel.Edit_ButtonEditTitle = string.IsNullOrEmpty(componentTopPanel.CreateForm_ButtonTitle)
                                                          ? componentTopPanel.CreateForm_ButtonText
                                                          : componentTopPanel.CreateForm_ButtonTitle;
                    }
                    else
                    {
                        componentTopPanel.Edit_ButtonEditDisabled = "disabled";
                        componentTopPanel.Edit_ButtonEditTitle = "Anda tidak mempunyai akses";
                    }
                }

            }

            //// Optionally, you can assign the cookie value to the model or use it as needed
            //componentTopPanel.USER_ID = USER_ID ?? "Default Value";


            return await Task.FromResult((IViewComponentResult)View("ComponentTopPanel", componentTopPanel));
        }

    }

}
