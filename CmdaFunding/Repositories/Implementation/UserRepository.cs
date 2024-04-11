using CmdaFunding.Commons.Enums;
using CmdaFunding.Controllers;
using CmdaFunding.Data;
using CmdaFunding.Models;
using CmdaFunding.Repositories.Interface;
using CmdaFunding.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using System.Security.Cryptography;
using System.Text;
using TechTalk.SpecFlow.CommonModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CmdaFunding.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private FundingContext _dbContext;
        public UserRepository(FundingContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserMaster> UserSaveAsync(LoginViewModel loginViewModel)
        {
            UserMaster user = new UserMaster();
            user.UserName = loginViewModel.username;
            user.CreatedDate = DateTime.Now;
            user.Password = HashPassword(loginViewModel.password);
            _dbContext.UserMaster.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;

        }

        public async Task<Message> UserSaveAsyncVerify(LoginViewModel loginViewModel)
        {
            Message msg = new Message();

            var encryptPwd = HashPassword(loginViewModel.password);

            var result = await _dbContext.UserMaster
             .Where(i => i.UserName == loginViewModel.username && i.Password == encryptPwd)
             .ToListAsync();

            if (result.Count == 0 || result == null)
            {
                msg.message = "Invalid Username or Password";
            }
            else
            {
                msg.message = "Login Successful";
            }
            return msg;
        }
        

        private string HashPassword(string password)
        {
            using (SHA512 hmac = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hasbytes = hmac.ComputeHash(bytes);
                return Convert.ToBase64String(hasbytes);

            }
        }

        

        public async Task<List<MenuRecordViewModel>> MenuList(LoginViewModel login)
        {
            List<MenuRecordViewModel> mList = new List<MenuRecordViewModel>();

            int uId;
            var encryptPwd = HashPassword(login.password);
            var data = await _dbContext.UserMaster
                       .FirstOrDefaultAsync(i => i.UserName == login.username && i.Password == encryptPwd);

            if (data != null)
            {
                uId = data.UserId;
                var userMenuMappings = await _dbContext.UserMenuMapping
                                       .Where(i => i.UserId == uId)
                                       .ToListAsync();

                var menuIds = userMenuMappings.Select(mm => mm.MenuId).ToList();

                var menuRecords = await _dbContext.MenuMaster
                    .Where(m => menuIds.Contains(m.MenuId))
                    //.OrderBy(i=>i.Sequence)                                       //Doubt
                    //.OrderByDescending(i=>i.Sequence)
                    .OrderBy(i => i.ParentMenuId) // Sort by ParentMenuId first
                    .ThenBy(i => i.MenuId)
                    .ToListAsync();

                // Group menu records by parent menu ID
                var groupedMenus = menuRecords.GroupBy(m => m.ParentMenuId);

                foreach (var group in groupedMenus)
                {
                    var parentMenuId = group.Key;
                    var parentMenu = mList.FirstOrDefault(m => m.MenuId == parentMenuId);
                    if (parentMenu == null)
                    {
                        parentMenu = new MenuRecordViewModel();
                        parentMenu.MenuId = parentMenuId.GetValueOrDefault();
                        parentMenu.SubMenu = new List<SubMenuRecordViewModel>();
                        if(parentMenu.MenuId==0)
                        {

                        }
                        else if(parentMenu.MenuId==3)
                        {
                            parentMenu.MenuName = "GatewayReports";
                            parentMenu.Type = "M";
                            mList.Add(parentMenu);
                        }
                        else if(parentMenu.MenuId==9) 
                        {
                            parentMenu.MenuName = "MISReports";
                            parentMenu.Type = "M";
                            mList.Add(parentMenu);
                        }
                        
                    }

                    foreach (var menu in group)
                    {
                        if (menu.MenuType == "S")
                        {
                            // Add to SubMenu if Type is 'S'
                            SubMenuRecordViewModel subMenu = new SubMenuRecordViewModel
                            {
                                MenuId = menu.MenuId,
                                MenuName = menu.MenuName,
                                Type = menu.MenuType,
                                Url = menu.URL,
                                ParentMenuId = menu.ParentMenuId
                            };
                            parentMenu.SubMenu.Add(subMenu);
                        }
                        else
                        {
                            // Add to parent menu if not a sub-menu
                            MenuRecordViewModel m = new MenuRecordViewModel
                            {
                                MenuId = menu.MenuId,
                                MenuName = menu.MenuName,
                                Type = menu.MenuType,
                                Url = menu.URL,
                                ParentMenuId = menu.ParentMenuId
                            };
                            mList.Add(m);
                        }
                    }
                }
            }

            return mList;
        }
        public async Task<IEnumerable<string>> AllUserListAsync()
        {
            var usr = await _dbContext.UserMaster
                            .OrderBy(u => u.UserId)
                            .Select(u => $"{u.UserId} - {u.UserName}")
                            .ToListAsync();

            return usr;
        }
        public async Task<Message> SaveUserAccessAysnc(UserAccessViewModel access)
        {
            Message msg = new Message();
            if (access.UserId != 0)
            {
                if (access.UserMaster==1)                            //MainMenu -Start
                {
                    UserMenuMapping um = new UserMenuMapping();
                    um.UserId = access.UserId;
                    um.MenuId = (int)MenuType.UserMaster;
                    _dbContext.UserMenuMapping.Add(um);                    
                    
                }
                if (access.UserAccessRights == 1)
                {
                    UserMenuMapping um = new UserMenuMapping();
                    um.UserId = access.UserId;
                    um.MenuId = (int)MenuType.UserAccessRights;
                    _dbContext.UserMenuMapping.Add(um);
                    
                }
                if (access.ChangePassword == 1)
                {
                    UserMenuMapping um = new UserMenuMapping();
                    um.UserId = access.UserId;
                    um.MenuId = (int)MenuType.ChangePassword;
                    _dbContext.UserMenuMapping.Add(um);
                    
                }                                                           //MainMenu - End

                foreach (var gatereport in access.GatewaywiseReports)       //Sub-Menu - Start
                {
                    if(gatereport.Transaction ==1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.Transaction;
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                    if (gatereport.Header == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.Header;
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                    if (gatereport.LocalBody == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.LocalBody;
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                    if (gatereport.District == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.District;
                        _dbContext.UserMenuMapping.Add(um);
                      
                    }
                    if (gatereport.Applicant == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.Applicant;
                        _dbContext.UserMenuMapping.Add(um);
                      
                    }

                }
                foreach(var misreport in access.MISReports)
                {
                    if (misreport.Header == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.MISHeader;
                        _dbContext.UserMenuMapping.Add(um);
                       
                    }
                    if (misreport.LocalBody == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.MISLocalBody;
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                    if (misreport.District == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.MISDistrict;
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                    if (misreport.Applicant == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.MISApplicant;
                        _dbContext.UserMenuMapping.Add(um);
                        await _dbContext.SaveChangesAsync();
                    }
                    if (misreport.FileList == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.FileList;
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                    if (misreport.FileDetails == 1)
                    {
                        UserMenuMapping um = new UserMenuMapping();
                        um.UserId = access.UserId;
                        um.MenuId = (int)MenuType.FileDetails; 
                        _dbContext.UserMenuMapping.Add(um);
                        
                    }
                }                                                                  //SubMenu - End
                msg.message ="User Access Rights Save Successfull";
                await _dbContext.SaveChangesAsync();
            }
            else 
            {
                msg.message = "Invalid User";
            }
            return msg;
        }

        public async Task<UserMaster> UserChangePassword(ChangePasswordViewModel chngpwd)
        {
            var oldEncryptPwd = HashPassword(chngpwd.oldpassword);

            var data = await _dbContext.UserMaster
                       .FirstOrDefaultAsync(i => i.UserId == chngpwd.userId
                       && i.UserName == chngpwd.username
                       && i.Password == oldEncryptPwd);

            if (data != null)
            {
                data.Password = HashPassword(chngpwd.password);
                data.CreatedDate = DateTime.Now;
                await _dbContext.SaveChangesAsync();
                return data;

            }
            return null;
        }

//------------------------------------------ Api Not Implemented ----------------------------------------------------
        public async Task<IEnumerable<UserMaster>> GetAllUserList()
        {
            return await _dbContext.UserMaster.ToListAsync();
        }

        public async Task<UserMaster> GetUserListByUserId(int id)
        {
            var data = await _dbContext.UserMaster.FirstOrDefaultAsync(i => i.UserId == id);
            if(data!=null)
            {
                return data;
            }
            else 
            { 
                return null; 
            }
            
        }

        public async Task<UserMaster> UserUpdateAsync(UserMaster usr)
        {
            var data = await _dbContext.UserMaster.FirstOrDefaultAsync(i => i.UserId == usr.UserId);
            if(data!=null) 
            {
                
                data.UserName = usr.UserName;
                data.CreatedDate = DateTime.Now;
                data.Password = HashPassword(usr.Password);
                await _dbContext.SaveChangesAsync();
                return data;
            
            }
            return null;
        }
    }
}
