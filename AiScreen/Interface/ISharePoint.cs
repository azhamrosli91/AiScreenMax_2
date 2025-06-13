using MaxSystemWebSite.Models.SETTING;
using MaxSystemWebSite.Models.EMAIL;
using Microsoft.Graph.Models;


namespace MaxSys.Interface
{
    public interface ISharePoint
    {
        void InitGraph(SETTING_EMAIL settings);
        #region "Employee Information"
            Task<(bool success, string message, ListCollectionResponse? data)> GetList(string _siteID);
            Task<(bool success, string message, List<SP_EmployeeInformation> data)> GetEmployeeInformation(string _siteID, string listID);
            Task<(bool success, string message, SP_EmployeeInformation data)> GetEmployeeInformationDetail(string _siteID, string listID, string email);
        #endregion

        #region "LEAVE"
            Task<(bool success, string message, List<SP_LeaveHistory> data)> GetLeaveHistory(string _siteID, string listID, string email);
            Task<(bool success, string message, List<SP_LeaveHistory> data)> GetAllLeaveHistory(string _siteID, string listID);
            Task<(bool success, string message, SP_LeaveInformation data)> GetLeaveInformation(string _siteID, string listID, string email);
        #endregion

        #region "PUBLIC HOLIDAY"
            Task<(bool success, string message, List<SP_PublicHoliday> data)> GetPublicHoliday(string _siteID, string listID);
        #endregion

        #region "OTHERS"
            Task<(bool success, string message, List<SP_CompanySA> data)> GetCompanySA(string _siteID, string listID);
        #endregion
    }
}
