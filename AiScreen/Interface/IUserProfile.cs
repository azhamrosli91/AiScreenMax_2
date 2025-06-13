using MaxSystemWebSite.Models.SETTING;
using MaxSystemWebSite.Models.USER;

namespace MaxSys.Interface
{
    public interface IUserProfile
    {
        void InitGraph(SETTING_EMAIL settings);
        Task<(bool success, string message, string? base64Image)> GetUserProfilePhotoAsync(string userEmail);
        Task<(bool success, string message, List<USER_PROFILE> ListData)> GetUserList();
    }

}
