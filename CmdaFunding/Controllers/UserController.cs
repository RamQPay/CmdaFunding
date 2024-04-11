using CmdaFunding.Data;
using CmdaFunding.Models;
using CmdaFunding.Repositories.Interface;
using CmdaFunding.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Nest;
using System.Security.Cryptography;
using System.Text;
using TechTalk.SpecFlow.CommonModels;

namespace CmdaFunding.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;                   //With Repository Pattern
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("UserRegister")]                                                    // UserRegistration-Save
        public async Task<UserMaster> SaveAsync(LoginViewModel loginViewModel)
        {
            return await _userRepository.UserSaveAsync(loginViewModel);
        }

        [HttpPost("UserVerification")] //1.
        public async Task<Message> SaveAsyncVerify(LoginViewModel loginViewModel)   //User-Verification-PostMethod
        {
            return await _userRepository.UserSaveAsyncVerify(loginViewModel);
        }

        [HttpPost("VerifyUserMenu")] //2.
        public async Task<List<MenuRecordViewModel>> MenuList(LoginViewModel login)
        {
            return await _userRepository.MenuList(login);
        }        

        [HttpGet("UserList")]                                               //Get AllUser - DropDown
        public async Task<IEnumerable<string>> UserList()
        {
            return await _userRepository.AllUserListAsync();
        }

        [HttpPost("UserAccessSave")]                       //To Save UserAccess Or Giving Rights to User
        public async Task<Message> SaveUserAccess(UserAccessViewModel access)
        {
            return await _userRepository.SaveUserAccessAysnc(access);
        }

        [HttpPost("ChangePassword")]                                                    // UserRegistration-Save
        public async Task<UserMaster> ChangePassword(ChangePasswordViewModel chngpwd)
        {
            return await _userRepository.UserChangePassword(chngpwd);
        }


//-----------------------------------------------Api Not Implemented in Ui ------------------------------------------------------------------------

        [HttpGet("GetAllUser")]                                                     // GetAllUser-List
        public async Task<IEnumerable<UserMaster>> GetAllUser()
        {
            return await _userRepository.GetAllUserList();
        }

        [HttpGet("GetUserListByUserID")]                                            // GetUserList- byUserID
        public async Task<UserMaster> GetUserById(int id)
        {
            return await _userRepository.GetUserListByUserId(id);
        }

        [HttpPost("UserUpdate")]                                                    // UserRegistration-Update
        public async Task<UserMaster> UpdateAsync(UserMaster usr)
        {
            return await _userRepository.UserUpdateAsync(usr);
        }
        
    }
}
//--------------------------------------------------------------------------------------------------------------
