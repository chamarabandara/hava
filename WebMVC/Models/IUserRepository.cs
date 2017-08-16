
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Principal;
using WebMVC.ModelViews;

namespace GlobalBatterier.Api.Models
{
    public interface IUserRepository
    {
        //JObject GetGroupList(GroupListViewModel vm);
        //JObject GetGroupList(int userId);
        JObject GetGroupRoleList(int groupId);
        JObject GetRoleList();
        int GetUserIdByUserName(string userName);
        //int GetEmployeeNoByUserId(int userId);
        //JObject GetUserList(UserSearchListViewModel vm);
        List<string> GetUserRolelist(string userName);
        //AppUserViewModel GetUserViewModel(string userName);
    }
}