using BaseSQL.Interface;
using Component_TableListing.Interface;
using Component_TableListing.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component_TableListing.Services
{
    public class TableService : ITable
    {
        private readonly IDapper _dapper;

        public TableService(IDapper dapper)
        {
            _dapper = dapper;
        }
        public async Task<List<COM_TABLE>> GetTableDataAsync()
        {
            // Replace with actual data fetching logic
            return await Task.FromResult(new List<COM_TABLE>
            {
                new COM_TABLE { /* Initialize with sample data */ },
                new COM_TABLE { /* Initialize with sample data */ }
            });
        }
        public async Task<(bool status, string message, COM_TABLE? model)> GenerateTableDataComponentAsync(string ConnectionString,int COM_TABLE_ID = 0) 
        {
            try
            {
                _dapper.SetConnectionString(ConnectionString);

                (bool status, string message, COM_TABLE model)  returnModel = await _dapper.PSP_COMMON_DAPPER_SINGLE<COM_TABLE>("PSP_COM_TABLE", System.Data.CommandType.StoredProcedure, new { COM_TABLE_ID, TYPE = 0 } );

                if (returnModel.status == true && returnModel.model != null) 
                {
                    (bool status, string message, List<COM_TABLE_D> model) returnModel_D = await _dapper.PSP_COMMON_DAPPER<COM_TABLE_D>("PSP_COM_TABLE", System.Data.CommandType.StoredProcedure, new { COM_TABLE_ID, TYPE = 1 });
                    if (returnModel_D.status == true && returnModel_D.model != null)
                    {
                        returnModel.model.COM_TABLE_D = returnModel_D.model;
                    }

                    if (!string.IsNullOrEmpty(returnModel.model.LOAD_STOR_PROC_PARAM)) {

                        returnModel.model.COM_STORE_PROC_PARAM = new List<COM_STORE_PROC_PARAM>();

                        List<COM_STORE_PROC_PARAM> parameters = JsonConvert.DeserializeObject<List<COM_STORE_PROC_PARAM>>(returnModel.model.LOAD_STOR_PROC_PARAM);

                        if (parameters != null && parameters.Count > 0) {
                            foreach (var param in parameters)
                            {
                                returnModel.model.COM_STORE_PROC_PARAM.Add(param);
                            }
                        }

                    }
                }

                return (true, "OK", returnModel.model);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }
        public (bool status, string message, ComponentTableModel? model) GenerateTableHeaderComponent(COM_TABLE table) 
        {
            try
            {
                ComponentTableModel componentTableModel = new ComponentTableModel();
                if (table == null)
                {
                    return (false, "Table not found", null);
                }

                componentTableModel.TableID = $"TblComponent_{table.COM_TABLE_ID.ToString()}";
                componentTableModel.TableName = table.TABLE_NAME;
                componentTableModel.Class = table.CLASS;
                componentTableModel.Field_ID = table.FIELD_ID_COLUMN;
                componentTableModel.ViewPath = table.VIEW_PATH;
                componentTableModel.PanelStatusField = table.PANEL_STATUS_FIELD;
                componentTableModel.PanelStatusVisible = table.PANEL_STATUS_VISIBLE;
                componentTableModel.PanelStatusTitle = table.PANEL_STATUS_TITLE;
                componentTableModel.LoadType = table.LOAD_TYPE;
                componentTableModel.AuditTrailVisible = table.AUDIT_TRAIL_VISIBLE;
                componentTableModel.DeleteRecordVisible = table.DELETE_RECORD_VISIBLE;
                componentTableModel.Visible_View = table.VISIBLE_VIEW;
                componentTableModel.Export_pdf_rotation = table.EXPORT_PDF;
                componentTableModel.Export_pdf_Paper_Size = table.EXPORT_PAPER;
                componentTableModel.Export_pdf_Title = table.EXPORT_TITLE;

                componentTableModel.AjaxInfo_FetchData = new ComponentTable_AJAXModel(table.AJAX_FETCHDATA_URL, table.AJAX_FETCHDATA_TYPE,
                    table.AJAX_FETCHDATA_DATA, table.AJAX_FETCHDATA_SUCCESS_URL, table.AJAX_FETCHDATA_SUCCESS_FAILED);
                componentTableModel.AjaxInfo_DeleteData = new ComponentTable_AJAXModel(table.AJAX_DELETEDATA_URL, table.AJAX_DELETEDATA_TYPE,
                    table.AJAX_DELETEDATA_DATA, table.AJAX_DELETEDATA_SUCCESS_URL, table.AJAX_DELETEDATA_SUCCESS_FAILED);
                
                
                if (componentTableModel.listComponentTable_ColumnModels == null)
                {
                    componentTableModel.listComponentTable_ColumnModels = new List<ComponentTable_ColumnModel>();
                }

                if (table.COM_TABLE_D == null || table.COM_TABLE_D.Count == 0)
                {
                    return (false, "Table detail(s) not found", null);    
                }

                componentTableModel.listComponentTable_ColumnModels.Add(new ComponentTable_ColumnModel(0, "ID#19471",
                        table.FIELD_ID_COLUMN, "", "", "","number",
                        false, true, ""));

                foreach (COM_TABLE_D item in table.COM_TABLE_D)
                {
                    
                    componentTableModel.listComponentTable_ColumnModels.Add(new ComponentTable_ColumnModel(item.SEQ ?? 0, item.COLUMN_TITLE,
                        item.COLUMN_NAME, item.STYLE, item.CLASS, item.WIDTH_PX.ToString() + "px",item.INPUT_TYPE,
                        item.ALLOW_FILTER ?? true, item.VISIBLE ?? true, item.CUSTOM_COMPONENT,item.ALLOW_EXPORT ?? true));
                }

                if (table.VISIBLE_VIEW == true) { 
                    componentTableModel.listComponentTable_ColumnModels.Add(new ComponentTable_ColumnModel(table.COM_TABLE_D.Count, "View",
                            table.FIELD_ID_COLUMN, "", "", "", "number",
                            false, true, ""));

                }
                return (true, "OK", componentTableModel);

            }
            catch (Exception ex)
            {
                return (false, ex.Message, null);
            }
        }

        public (bool status, string message, ExpandoObject model) ConvertToDynamicModel(List<COM_TABLE_D> tableDList)
        {
            try
            {
                // Validate the input list
                if (tableDList == null || tableDList.Count == 0)
                {
                    return (false, "Table list cannot be empty", null);
                }

                IDictionary<string, object> expando = new ExpandoObject();

                foreach (var column in tableDList)
                {
                    // Determine the property name
                    string propertyName = column.COLUMN_NAME;

                    // Get the default value for the given data type
                    var (status, message, propertyValue) = GetDefaultValue(column.COLUMN_DATATYPE);

                    // If GetDefaultValue returns an error, handle it
                    if (!status)
                    {
                        return (false, $"Error processing column '{propertyName}': {message}", null);
                    }

                    // Add the property to the ExpandoObject
                    expando[propertyName] = propertyValue;
                }

                return (true, "OK", (ExpandoObject)expando);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        public (bool status, string message, object model) GetDefaultValue(string dataType)
        {
            try
            {
                // Check if the dataType is null or empty
                if (string.IsNullOrEmpty(dataType?.Trim()))
                {
                    return (false, "Data type cannot be empty", null);
                }

                // Trim the input to remove any leading or trailing spaces
                dataType = dataType.Trim().ToLower();

                // Determine the default value based on the data type using a traditional switch statement
                object obj;
                switch (dataType)
                {
                    case "int":
                    case "bigint":
                        obj = default(int);
                        break;
                    case "decimal":
                        obj = default(decimal);
                        break;
                    case "double":
                        obj = default(double);
                        break;
                    case "float":
                        obj = default(float);
                        break;
                    case "varchar":
                    case "nvarchar":
                    case "uniqueidentifier":
                        obj = string.Empty; // Use empty string instead of null for string types
                        break;
                    case "datetime":
                        obj = default(DateTime);
                        break;
                    case "date":
                        obj = default(DateTime);
                        break;
                    case "bit":
                        obj = default(bool);
                        break;
                    default:
                        obj = null; // Fallback to null for unknown types
                        break;
                }

                // Check if the data type was unrecognized
                if (obj == null)
                {
                    return (false, $"Unrecognized data type: '{dataType}'", null);
                }

                return (true, "OK", obj);
            }
            catch (Exception ex)
            {

                return (false, ex.Message, null);
            }
        }
       
        public ConversionResult ConvertDefaultValue(string dataType,string value)
        {
            try
            {
                // Check if the dataType is null or empty
                if (string.IsNullOrEmpty(dataType?.Trim()))
                {
                    return new ConversionResult { Status = false, Message = "Data type cannot be empty", Model = null };
                }

                // Trim the input to remove any leading or trailing spaces
                dataType = dataType.Trim().ToLower();

                // Initialize object to hold the converted value
                object obj;

                // Determine the default value based on the data type using a switch statement
                switch (dataType)
                {
                    case "int":
                        if (int.TryParse(value, out int intValue))
                        {
                            obj = intValue;
                        }
                        else
                        {
                            return new ConversionResult { Status = false, Message = $"Cannot convert '{value}' to {dataType}", Model = null };
                        }
                        break;

                    case "decimal":
                        if (decimal.TryParse(value, out decimal decimalValue))
                        {
                            obj = decimalValue;
                        }
                        else
                        {
                            return new ConversionResult { Status = false, Message = $"Cannot convert '{value}' to {dataType}", Model = null };
                        }
                        break;

                    case "double":
                    case "float":
                        if (double.TryParse(value, out double doubleValue))
                        {
                            obj = doubleValue;
                        }
                        else
                        {
                            return new ConversionResult { Status = false, Message = $"Cannot convert '{value}' to {dataType}", Model = null };
                        }
                        break;

                    case "varchar":
                    case "nvarchar":
                    case "uniqueidentifier":
                        obj = value ?? string.Empty; // Use empty string if value is null
                        break;

                    case "datetime":
                        if (DateTime.TryParse(value, out DateTime dateTimeValue))
                        {
                            obj = dateTimeValue;
                        }
                        else
                        {
                            return new ConversionResult { Status = false, Message = $"Cannot convert '{value}' to {dataType}", Model = null };
                        }
                        break;

                    case "bit":
                        if (bool.TryParse(value, out bool boolValue))
                        {
                            obj = boolValue;
                        }
                        else
                        {
                            return new ConversionResult { Status = false, Message = $"Cannot convert '{value}' to {dataType}", Model = null };
                        }
                        break;

                    default:
                        return new ConversionResult { Status = false, Message = $"Unrecognized data type: '{dataType}'", Model = null };
                }

                // Return success if conversion was successful
                return new ConversionResult { Status = true, Message = "OK", Model = obj };
            }
            catch (Exception ex)
            {

                return new ConversionResult { Status = false, Message = ex.Message, Model = null};
            }
        }
    }
}
