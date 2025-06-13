using Component_TableListing.Models;
using System.Dynamic;
using static Component_TableListing.Services.TableService;

namespace Component_TableListing.Interface
{
    public interface ITable
    {
        Task<List<COM_TABLE>> GetTableDataAsync();
        Task<(bool status, string message, COM_TABLE? model)> GenerateTableDataComponentAsync(string ConnectionString, int MM_TABLE_ID = 0);
        (bool status, string message, ComponentTableModel? model) GenerateTableHeaderComponent(COM_TABLE table);

        (bool status, string message, ExpandoObject model) ConvertToDynamicModel(List<COM_TABLE_D> tableDList);
        (bool status, string message, object model) GetDefaultValue(string dataType);
        ConversionResult ConvertDefaultValue(string dataType, string value);
    }
 
}
