namespace SmartTemplateCore.Models.Common
{
    public class ExcelExportRequest
    {
        public string Filename { get; set; }
        public List<Dictionary<string, object>> Data { get; set; }
    }
}
