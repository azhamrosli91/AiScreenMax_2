using Base.Model;
using BaseSQL.Interface;
using Component_TableListing.Interface;
using Component_TableListing.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Dynamic;
using System.Reflection;

namespace Component_TableListing.Controllers.Components
{
    [ViewComponent(Name = "ComponentTableV1")]
    public class ComponentTableV1ViewComponent : ViewComponent
    {
        private readonly ITable _tableService;
        private IConfiguration _configuration;
        private readonly IDapper _dapper;
        // Constructor with dependency injection
        public ComponentTableV1ViewComponent(ITable tableService, IConfiguration configuration, IDapper dapper)
        {
            _tableService = tableService;
            _configuration = configuration;
            _dapper = dapper;
        }
        public async Task<IViewComponentResult> InvokeAsync(object param)
        {
            try
            {
                if (param is ComponentTableModel componentTableModel)
                {
                    // Handle case where the parameter is ComponentTableModel
                    if (componentTableModel == null)
                    {
                        componentTableModel = new ComponentTableModel();
                    }

                    return View("ComponentTableV2", componentTableModel);
                }
                else if (param is int COM_TABLE_ID)
                {
                    // Handle case where the parameter is an integer (COM_TABLE_ID)
                    string connectionString = _configuration["ConnectionString_Dummy"] ?? "";

                    if (string.IsNullOrEmpty(connectionString))
                    {
                        ViewData["ErrorMessage"] = "Connection string is empty";
                        return View("error");
                    }

                    (bool status, string message, COM_TABLE model) returnTableModel = await _tableService.GenerateTableDataComponentAsync(connectionString, COM_TABLE_ID);

                    if (returnTableModel.status == false || returnTableModel.model == null)
                    {
                        ViewData["ErrorMessage"] = returnTableModel.message;
                        return View("error");
                    }

                    (bool status, string message, ComponentTableModel? model) returnComponentModel = _tableService.GenerateTableHeaderComponent(returnTableModel.model);

                    if (returnComponentModel.status == false || returnComponentModel.model == null)
                    {
                        ViewData["ErrorMessage"] = returnComponentModel.message;
                        return View("error");
                    }

                    if (returnTableModel.model.LOAD_TYPE == 2)
                    {
                        //Fully auto from front end, backend store proc
                        returnTableModel.model.AJAX_FETCHDATA_URL = "/TableGenerator/Get";
                        returnTableModel.model.AJAX_DELETEDATA_URL = "/TableGenerator/Delete";

                        (bool status, string message, ExpandoObject model) returnDynamicModel = _tableService.ConvertToDynamicModel(returnTableModel.model.COM_TABLE_D);

                        if (returnDynamicModel.status == false)
                        {
                            ViewData["ErrorMessage"] = returnDynamicModel.message;
                            return View("error");
                        }

                        (bool status, string message, dynamic model) returnDynamicData = await _dapper.PSP_COMMON_DAPPER<dynamic>("PSP_COM_TABLE_DATA_EXEC", System.Data.CommandType.StoredProcedure, new { COM_TABLE_ID });

                        if (returnDynamicData.status == false)
                        {
                            ViewData["ErrorMessage"] = returnDynamicData.message;
                            return View("error");
                        }
                        // Ensure correct assignment based on the type of data returned
                        if (returnDynamicData.model is List<object> list && list.Count > 0)
                        {
                            returnComponentModel.model.DataModel = new List<dynamic>();

                            foreach (var item in list)
                            {
                                if (item is IDictionary<string, object> dictionary) // Check if item can be treated as dictionary
                                {
                                    ExpandoObject expando = new ExpandoObject();
                                    var expandoDict = (IDictionary<string, object>)expando;

                                    foreach (var kvp in dictionary)
                                    {
                                        expandoDict[kvp.Key] = kvp.Value; // Copy each property to expando
                                    }

                                    returnComponentModel.model.DataModel.Add(expando);
                                }
                                else
                                {
                                    ViewData["ErrorMessage"] = "Item is not in the expected format.";
                                    return View("error");
                                }
                            }
                        }
                        else if (returnDynamicData.model is ExpandoObject expando)
                        {
                            returnComponentModel.model.DataModel = new List<dynamic> { expando };
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "Type mismatch or unexpected result";
                            return View("error"); // Type mismatch or unexpected result
                        }

                    }
                    else if (returnTableModel.model.LOAD_TYPE == 1)
                    {
                        //Partial Auto, Auto make backend but store need to provide
                        returnTableModel.model.AJAX_FETCHDATA_URL = "/TableGenerator/Get";
                        returnTableModel.model.AJAX_DELETEDATA_URL = "/TableGenerator/Delete";

                        (bool status, string message, ExpandoObject model) returnDynamicModel = _tableService.ConvertToDynamicModel(returnTableModel.model.COM_TABLE_D);

                        if (returnDynamicModel.status == false)
                        {
                            ViewData["ErrorMessage"] = returnDynamicModel.message;
                            return View("error");
                        }

                        //will load data from store proc using dapper and pass to dynamic model
                        var parameters = new DynamicParameters();
                        if (returnTableModel.model.COM_STORE_PROC_PARAM != null) {
                            foreach (var itemParam in returnTableModel.model.COM_STORE_PROC_PARAM)
                            {
                                if (!string.IsNullOrEmpty(itemParam.PARAMETER_NAME))
                                {
                                    // Call ConvertDefaultValue and get the result
                                    var conversionResult = _tableService.ConvertDefaultValue(itemParam.DATA_TYPE, itemParam.DEFAULT_VALUE);

                                    // Check if conversion was successful
                                    if (conversionResult.Status)
                                    {
                                        // Add the converted value to the parameters
                                        parameters.Add(itemParam.PARAMETER_NAME, conversionResult.Model);
                                    }
                                    else
                                    {
                                        // Handle conversion error
                                        ViewData["ErrorMessage"] = $"Error converting parameter '{itemParam.PARAMETER_NAME}': {conversionResult.Message}";
                                        return View("error");
                                    }
                                }
                            }
                        }

                        (bool status, string message, dynamic model) returnDynamicData = await _dapper.PSP_COMMON_DAPPER<dynamic>(returnTableModel.model.LOAD_STOR_PROC, System.Data.CommandType.StoredProcedure, parameters);

                        if (returnDynamicData.status == false)
                        {
                            ViewData["ErrorMessage"] = returnDynamicData.message;
                            return View("error");
                        }
                        // Ensure correct assignment based on the type of data returned
                        if (returnDynamicData.model is List<object> list && list.Count > 0)
                        {
                            returnComponentModel.model.DataModel = new List<dynamic>();

                            foreach (var item in list)
                            {
                                if (item is IDictionary<string, object> dictionary) // Check if item can be treated as dictionary
                                {
                                    ExpandoObject expando = new ExpandoObject();
                                    var expandoDict = (IDictionary<string, object>)expando;

                                    foreach (var kvp in dictionary)
                                    {
                                        expandoDict[kvp.Key] = kvp.Value; // Copy each property to expando
                                    }

                                    returnComponentModel.model.DataModel.Add(expando);
                                }
                                else
                                {
                                    ViewData["ErrorMessage"] = "Item is not in the expected format.";
                                    return View("error");
                                }
                            }
                        }
                        else if (returnDynamicData.model is ExpandoObject expando)
                        {
                            returnComponentModel.model.DataModel = new List<dynamic> { expando };
                        }
                        else
                        {
                            ViewData["ErrorMessage"] = "Type mismatch or unexpected result";
                            return View("error"); // Type mismatch or unexpected result
                        }

                        // Assign ExpandoObject to DataModel list


                    }
                    else
                    {
                        //Manual
                        returnComponentModel.model.DataModel = new List<dynamic> { };
                    }

                    //Access Level
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
                            if (item.DELETE_RIGHT <= Convert.ToInt32(ACCESS_LEVEL))
                            {
                                returnComponentModel.model.Delete_Right = "";
                            }
                            else
                            {
                                returnComponentModel.model.Delete_Right = "disabled";
                            }

                        }

                    }

                    return View("ComponentTableV1", returnComponentModel.model);
                }
                ViewData["ErrorMessage"] = "Invalid parameter not correct.";
                return View("error");
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View("error");
            }
          
          
            
        }
    }
}
