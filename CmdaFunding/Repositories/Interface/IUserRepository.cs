
using CmdaFunding.Models;
using CmdaFunding.ViewModels;
using Microsoft.AspNetCore.Mvc;
using TechTalk.SpecFlow.CommonModels;

namespace CmdaFunding.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<UserMaster> UserSaveAsync(LoginViewModel loginViewModel);
        Task<Message> UserSaveAsyncVerify(LoginViewModel loginViewModel);
        Task<UserMaster> UserChangePassword(ChangePasswordViewModel chngpwd);
        Task<List<MenuRecordViewModel>> MenuList(LoginViewModel login);
        Task<Message> SaveUserAccessAysnc(UserAccessViewModel access);
        Task<IEnumerable<string>> AllUserListAsync();

//------------------------------------ Api Call Not Implemented in Ui ---------------------------------------------
        Task<IEnumerable<UserMaster>> GetAllUserList();
        Task<UserMaster> GetUserListByUserId(int id);        
        Task<UserMaster> UserUpdateAsync(UserMaster usr);
        
    }
}
