using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace MaxSys.Helpers
{
    public class CommonMethod
    {
        #region Convert datatable into normal listing
        /// <summary>
        /// Azham 01/2/2022
        /// List<T> ConvertToList<T> Convert Datatable to the model
        /// Warning!! Please do not use this function if data more than 100rows or Column More than 10
        /// </summary>
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            List<string> columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.ToLower()).ToList();
            PropertyInfo[] properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row => {
                var obj = Activator.CreateInstance<T>();
                Parallel.ForEach(properties, property =>
                {
                    if (columnNames.Contains(property.Name.ToLower()))
                    {
                        var value = ChangeType(row[property.Name], property.PropertyType);
                        property.SetValue(obj, value);
                    }
                });
                return obj;
            }).ToList();
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                t = Nullable.GetUnderlyingType(t);
            }

            return Convert.ChangeType(value, t);
        }
        public static string GenerateInvoiceNumber()
        {
            DateTime currentDate = DateTime.Now;
            string year = currentDate.ToString("yy");   // Extract last two digits of the year
            string month = currentDate.ToString("MM");  // Extract two-digit month
            string day = currentDate.ToString("dd");    // Extract two-digit day

            return $"INV{year}{month}{day}";
        }
        #endregion

        #region Collect searching parameter into SQL Query 
        /// <summary>
        /// Azham 01/2/2022
        /// Collect searching parameter and convert into SQL Query 
        /// </summary>
        public static string ConvertToSearchSQL(object obj, string DeletedColumnName)
        {
            string SQL = "";
            string probName = "";
            string PropertyType = "";
            string probval = "";
            Type typ = obj.GetType();

            foreach (var prop2 in typ.GetProperties())
            {

                if (prop2.Name.Contains("SEARCH") == true)
                {
                    probName = prop2.Name;
                    PropertyType = prop2.PropertyType.Name;
                    probval = string.Format("{0}", prop2.GetValue(obj));

                    if (prop2.GetValue(obj) != null && probval.ToString().Trim() != "")
                    {
                        if (PropertyType == "int" || PropertyType == "double" || PropertyType == "float" || PropertyType == "decimal")
                        {
                            SQL += string.Format(" AND {0}={1}", prop2.Name.Replace("SEARCH_", ""), prop2.GetValue(obj));
                        }
                        else if (PropertyType == "bool" || PropertyType == "Boolean")
                        {
                            if (DeletedColumnName == probName.Replace("SEARCH_", ""))
                            {
                                if (Convert.ToBoolean(prop2.GetValue(obj)) == false)
                                {
                                    SQL += " AND " + DeletedColumnName + " <>'5'";
                                }
                            }
                            else
                            {
                                SQL += string.Format(" AND {0}='{1}'", prop2.Name.Replace("SEARCH_", ""), prop2.GetValue(obj));
                            }
                        }
                        else
                        {
                            SQL += string.Format(" AND {0}='{1}'", prop2.Name.Replace("SEARCH_", ""), prop2.GetValue(obj));
                        }
                    }
                }

            }
            return SQL;
        }
        #endregion

        #region object type checking 
        /// <summary>
        /// Azham 07/11/2022
        /// cDate checking the string is date or not correct date format and will be return true or false
        /// </summary>
        public static bool isDate(string value)
        {
            try
            {
                DateTime dt;


                return DateTime.TryParse(value, out dt);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        /// <summary>
        /// Azham 07/11/2022
        /// isNumeric checking the string is number or not number and will be return true or false
        /// </summary>
        public static bool isNumeric(string value)
        {
            try
            {
                float output;
                return float.TryParse(value, out output);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
        public static string GetDurition(DateTime Open, DateTime? Close)
        {
            DateTime TmpClose = DateTime.Now;
            if (Close != null)
            {
                TmpClose = Convert.ToDateTime(Close);
            }

            TimeSpan duration = TmpClose - Open;

            if (duration.TotalMinutes < 60)
            {
                return $"Less a minutes";
            }
            else if (duration.TotalHours < 24)
            {
                return $"{Convert.ToInt32(duration.TotalHours).ToString()} hours and {Convert.ToInt32(duration.Minutes).ToString()} minutes";
            }
            else if (duration.TotalDays < 30)
            {
                return $"{Convert.ToInt32(duration.Days).ToString()} days and {Convert.ToInt32(duration.Hours).ToString()} hours";
            }
            else if (duration.TotalDays < 365)
            {
                int months = (int)(duration.TotalDays / 30);
                return $"{Convert.ToInt32(months).ToString()} months and {Convert.ToInt32(duration.Days).ToString()} days";
            }
            else
            {
                int years = (int)(duration.TotalDays / 365);
                return $"{Convert.ToInt32(years).ToString()} years and {Convert.ToInt32(duration.Days).ToString()} days";
            }
        }
        public static bool IsBase64String(string base64)
        {
            if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0)
            {
                return false;
            }

            // Regular expression to check for valid Base64 characters
            var base64Regex = new Regex(@"^[a-zA-Z0-9\+/]*={0,2}$", RegexOptions.None);

            if (!base64Regex.IsMatch(base64))
            {
                return false;
            }

            try
            {
                // Attempt to decode the string to see if it's valid Base64
                Convert.FromBase64String(base64);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static string Convert64baseToString(string key) 
        {
            string base64String;
            if (IsBase64String(key))
            {
                byte[] bytes = Convert.FromBase64String(key);
                base64String = System.Text.Encoding.UTF8.GetString(bytes);
            }
            else {
                base64String = key;
            }
           
            return base64String;
        }
        public static string? ConvertStringTo64base(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(key);
            string base64String = Convert.ToBase64String(bytes);
            return base64String;
        }
        #endregion
        #region "IO File Folder"
        public static (bool success, string newPath, string message) MoveFile(string originalFilePath,string destinationFolder) 
        {

            try
            {
                // Define the source path (current location of the file)
    
                string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", originalFilePath.TrimStart('/'));

                // Define the destination path (new location of the file)
                string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", destinationFolder, Path.GetFileName(sourcePath));
                // Ensure the destination directory exists
                string destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }
                
                // Move the file
                System.IO.File.Move(sourcePath, destinationPath);

                return (true, "/" + destinationFolder + "/" + Path.GetFileName(sourcePath), "Successfully move file" );
            }
            catch (Exception ex)
            {
                return (false, "", ex.Message.ToString());
            }
        }
        public static (bool success, string message) DeleteFile(string originalFilePath)
        {

            try
            {
                // Define the source path (current location of the file)
                string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), originalFilePath);

    
                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Delete(sourcePath);
                }

                return (false, "Successfully deleted file");
            }
            catch (Exception ex)
            {
                return (false, ex.Message.ToString());
            }
        }
        #endregion
        #region PRINT_PDF
        public static async Task<string> RenderViewToStringAsync(IServiceProvider serviceProvider, string viewPath, object model = null)
        {
            var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            var viewEngine = serviceProvider.GetRequiredService<ICompositeViewEngine>();
            var tempDataProvider = serviceProvider.GetRequiredService<ITempDataProvider>();
            var viewResult = viewEngine.FindView(actionContext, viewPath, isMainPage: false);
          
            if (!viewResult.Success)
            {
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "Views/DE_Payment_History/PrintReceipt2.cshtml");
                Console.WriteLine($"Expected view path: {physicalPath}");
                var searchedLocations = viewResult.SearchedLocations;
                var message = $"View '{viewPath}' not found. Searched in: {string.Join(", ", searchedLocations)}";
                Console.WriteLine(message);  // Log to console (or use any other logging mechanism)
                throw new FileNotFoundException($"View '{viewPath}' not found.");
            }

            var view = viewResult.View;
            using (var stringWriter = new StringWriter())
            {
                var viewDataDictionary = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(httpContext, tempDataProvider);
                var viewContext = new ViewContext(actionContext, view, viewDataDictionary, tempData, stringWriter, new HtmlHelperOptions());

                await view.RenderAsync(viewContext);
                return stringWriter.ToString();
            }
        }
        #endregion
        #region "DATAFILTER"
        public static List<SelectListItem> GetFilter_Month()
        {
            var malayCulture = new CultureInfo("ms-MY");
            var currentDate = DateTime.Now;

            var yearsWithMonths = Enumerable.Range(currentDate.AddYears(-1).Year, 2)
                .SelectMany(year => Enumerable.Range(1, 12)
                    .Select(month => new
                    {
                        Value = $"{year}-{month:D2}", // Format: yyyy-MM
                        Text = new DateTime(year, month, 1).ToString("MMMM yyyy", malayCulture), // Malay Month and Year
                        IsCurrentMonth = year == currentDate.Year && month == currentDate.Month // Check for current month
                    }))
                .OrderByDescending(item => item.Value) // Order by Value in descending order
                .Select(item => new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text,
                    Selected = item.IsCurrentMonth // Set selected if it's the current month
                })
                .ToList();

            return yearsWithMonths;
        }
        public static List<SelectListItem> GetFilter_Month(int totalMonths)
        {
            var malayCulture = new CultureInfo("ms-MY");
            var currentDate = DateTime.Now;

            var monthsList = Enumerable.Range(0, totalMonths) // Generate the last `totalMonths`
                .Select(offset => currentDate.AddMonths(-offset)) // Go back in time
                .Select(date => new
                {
                    Value = date.ToString("yyyy-MM"), // Format: yyyy-MM
                    Text = date.ToString("MMMM yyyy", malayCulture), // Malay Month and Year
                    IsCurrentMonth = date.Year == currentDate.Year && date.Month == currentDate.Month // Check for current month
                })
                .OrderByDescending(item => item.Value) // Ensure latest months appear first
                .Select(item => new SelectListItem
                {
                    Value = item.Value,
                    Text = item.Text,
                    Selected = item.IsCurrentMonth // Set selected if it's the current month
                })
                .ToList();

            return monthsList;
        }
        #endregion
    }
    public static class StringCipher
    {
       
    }
}
