using Base.Model;
using BaseModel.Models.Component;
using BaseSQL.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace MaxSys.Controllers.Components
{
    public class ComponentBottomPanelViewComponent : ViewComponent
    {
        private readonly IDapper _dapper;
        public ComponentBottomPanelViewComponent(IDapper dapper)
        {
            _dapper = dapper;
        }
        public virtual async Task<IViewComponentResult> InvokeAsync(ComponentBottomPanelModel componentBottomPanel)
        {
            if (componentBottomPanel == null) { componentBottomPanel = new ComponentBottomPanelModel(); }

            // Accessing cookies from the HttpContext
            var USER_ID = HttpContext.Request.Cookies["USER_ID"];
            var ID_MM_COMPANY = HttpContext.Request.Cookies["ID_MM_COMPANY"];
            var ACCESS_LEVEL = HttpContext.Request.Cookies["ACCESS_LEVEL"];
            // Access the current controller and action names
            var controllerName = HttpContext.Request.RouteValues["controller"]?.ToString();
            var actionName = HttpContext.Request.RouteValues["action"]?.ToString();

            (bool success, string message, List<AclResource> item) side_bar = await _dapper.PSP_COMMON_DAPPER<AclResource>("PSP_ACL_RESOURCE_CONTROL",
                CommandType.StoredProcedure, new { USER_ID, ID_MM_COMPANY, RESOURCE_CONTROLLER = controllerName });

            if (ICommonMethod.isNumeric(ACCESS_LEVEL.ToString()) == false)
            {
                ACCESS_LEVEL = "1";
            }
            if (side_bar.success == true && side_bar.item != null)
            {
                bool btnAdd = true;

                foreach (AclResource item in side_bar.item)
                {
                    if (item.ADD_RIGHT <= Convert.ToInt32(ACCESS_LEVEL))
                    {
                        componentBottomPanel.ButtonDisabled_Submit = "";
                        componentBottomPanel.ButtonTitle_Submit = string.IsNullOrEmpty(componentBottomPanel.ButtonTitle_Submit)
                                                          ? componentBottomPanel.ButtonText_Submit
                                                          : componentBottomPanel.ButtonTitle_Submit;
                        componentBottomPanel.ButtonDisabled_SaveAsDraft = "";
                        componentBottomPanel.ButtonTitle_SaveAsDraft = string.IsNullOrEmpty(componentBottomPanel.ButtonTitle_SaveAsDraft)
                                                          ? componentBottomPanel.ButtonText_SaveAsDraft
                                                          : componentBottomPanel.ButtonTitle_SaveAsDraft;
                    }
                    else
                    {
                        componentBottomPanel.ButtonDisabled_Submit = "disabled";
                        componentBottomPanel.ButtonTitle_Submit = "Anda tidak mempunyai akses";

                        componentBottomPanel.ButtonDisabled_SaveAsDraft = "disabled";
                        componentBottomPanel.ButtonTitle_SaveAsDraft = "Anda tidak mempunyai akses";
                    }


                    if (item.DELETE_RIGHT <= Convert.ToInt32(ACCESS_LEVEL))
                    {
                        componentBottomPanel.ButtonDisabled_Delete = "";
                        componentBottomPanel.ButtonTitle_Delete = string.IsNullOrEmpty(componentBottomPanel.ButtonTitle_Delete)
                                                          ? componentBottomPanel.ButtonText_Delete
                                                          : componentBottomPanel.ButtonTitle_Delete;
                    }
                    else
                    {
                        componentBottomPanel.ButtonDisabled_Delete = "disabled";
                        componentBottomPanel.ButtonTitle_Delete = "Anda tidak mempunyai akses";
                    }
                }

            }

            return await Task.FromResult((IViewComponentResult)View("ComponentBottomPanel", componentBottomPanel));
        }

    }

}
