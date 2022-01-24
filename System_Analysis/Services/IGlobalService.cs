using Chat.Web.ViewModels;
using System_Analysis.DTO;

namespace System_Analysis.Services
{
    public interface IGlobalService
    {
        Task<OtpResponseDTO> GetOtp(string mobileNumber);
        Task<LoginResult> Login(LoginInfo login);
        Task<UserViewModel> FindMember(string username);
        Task<bool> EditProfile(UserDTO user, Guid userId);
        Task<List<UserViewModel>> SearchMembers(string username);
        Task<UserViewModel> FindMemberByMobile(string mobileNumber);
    }
}