
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Configuration;
using GlobalBatterier.Api.Models;
using HavaBusiness;
using WebMVC.ModelViews;
using WebMVC.Common;

namespace WebMVC.Models
{

    public class UserRepository : IUserRepository
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private HAVA_DBModelEntities gb_db = new HAVA_DBModelEntities();

        //public JObject GetGroupList(GroupListViewModel vm)
        //{
        //    JObject returnObj = new JObject();
        //    JArray jsonArray = new JArray();

        //    //group List 
        //    var groupList = (from appGroup in gb_db.SearchGroups(vm.sortColumn, vm.sortOrder)
        //                     select appGroup).ToList();


        //    //To Remove
        //    foreach (var group in groupList)
        //    {

        //        JObject jsonList = new JObject();
        //        jsonList.Add("id", group.ID);

        //        ;
        //        jsonList.Add("name", group.Name);
        //        jsonList.Add("roles", group.Roles);
        //        jsonList.Add("checked", false);
        //        jsonArray.Add(jsonList);
        //    }

        //    returnObj.Add("data", jsonArray);
        //    returnObj.Add("totalRows", jsonArray.Count());

        //    return returnObj;
        //}


        
        //public JObject GetGroupList(int userId)
        //{
        //    JObject returnObj = new JObject();
        //    JArray jsonArray = new JArray();




        //    //group List 
        //    var groupList = (from appGroup in gb_db.GetAllGroups()
        //                     select appGroup).ToList();

        //    //group List 
        //    var selectedUser = (from appUser in db.Users
        //                        where appUser.Id == userId
        //                        select appUser).FirstOrDefault();

        //    ArrayList selectedGroupsArray = new ArrayList();
        //    if (selectedUser != null)
        //    {
        //        foreach (ApplicationUserGroup appGroup in selectedUser.Groups)
        //        {
        //            selectedGroupsArray.Add(appGroup.GroupId);

        //        }
        //    }
        //    foreach (var group in groupList)
        //    {
        //        JObject jsonList = new JObject();
        //        jsonList.Add("id", group.ID);
        //        jsonList.Add("name", group.Name);
        //        jsonList.Add("roles", group.Roles);
        //        if (selectedGroupsArray.Contains(group.ID))

        //        {
        //            jsonList.Add("checked", true);
        //        }
        //        else {
        //            jsonList.Add("checked", false);
        //        }

        //        jsonArray.Add(jsonList);
        //    }

        //    returnObj.Add("data", jsonArray);
        //    returnObj.Add("totalRows", jsonArray.Count());

        //    return returnObj;
        //}


        //public JObject GetUserList(UserSearchListViewModel vm)
        //{
        //    JObject returnObj = new JObject();
        //    JArray jsonArray = new JArray();
        //    string searchQry = Helper.GenerateSearchStatement(vm.searchData);
        //    //group List
        //    var userList = (from appGroup in gb_db.SearchUsers(vm.pageSize,vm.pageNumber,vm.sortColumn,vm.sortOrder, searchQry)
        //                    select appGroup).ToList();

        //    //To Remove
        //    foreach (var user in userList)
        //    {
        //        JObject jsonList = new JObject();
        //        jsonList.Add("id", user.Id);
        //        jsonList.Add("name", user.Name);
        //        jsonList.Add("email", user.Email);
        //        jsonList.Add("groups", user.UserGroups);

        //        jsonArray.Add(jsonList);
        //    }

        //    returnObj.Add("data", jsonArray);
        //    returnObj.Add("totalRows", jsonArray.Count());

        //    return returnObj;
        //}


        public JObject GetRoleList()
        {
            JArray jsonArray = new JArray();
            //role List
            var roleList = (from appRole in db.Roles
                            select appRole).ToList();


            foreach (var role in roleList)
            {
                JObject jsonList = new JObject();
                jsonList.Add("id", role.Id);
                jsonList.Add("name", role.Name);
                jsonArray.Add(jsonList);
            }
            JObject returnObj = new JObject();
            returnObj.Add("data", jsonArray);
            returnObj.Add("totalRows", jsonArray.Count());

            return returnObj;
        }
       

        public JObject GetGroupRoleList(int groupId)
        {
            JArray jsonArray = new JArray();
            //role List
            var roleList = (from appRole in db.Roles
                            select appRole).ToList();

            //group List 
            var selectedGroup = (from appUser in db.ApplicationGroups
                                 where appUser.Id == groupId
                                 select appUser).FirstOrDefault();

            ArrayList selectedRolesArray = new ArrayList();
            if (selectedGroup != null)
            {
                foreach (ApplicationRoleGroup appGroup in selectedGroup.Roles)
                {
                    selectedRolesArray.Add(appGroup.RoleId);

                }
            }
            foreach (var role in roleList)
            {
                JObject jsonList = new JObject();
                jsonList.Add("id", role.Id);
                jsonList.Add("name", role.Name);
                if (selectedRolesArray.Contains(role.Id))

                {
                    jsonList.Add("checked", true);
                }
                jsonArray.Add(jsonList);
            }
            JObject returnObj = new JObject();
            returnObj.Add("data", jsonArray);
            returnObj.Add("totalRows", jsonArray.Count());

            return returnObj;
        }

       

        public int GetUserIdByUserName(string userName)
        {
            JArray jsonArray = new JArray();
            int userId = 0;

            var userDetails = (from user in db.Users
                               where user.UserName == userName
                               select user).FirstOrDefault();

            if (userDetails != null)
                userId = userDetails.Id;

            return userId;
        }


        public List<string> GetUserRolelist(string userName)
        {
            using (var db = new ApplicationDbContext())
            {
                var userDetails = db.Users.FirstOrDefault(x => x.UserName == userName || x.Id == 6);

                var roles = from u in userDetails.Roles
                            from r in db.Roles.Where(x => x.Id == u.RoleId)
                            select r.Name;

                return roles.ToList();

            }


        }
        public string GetNameByUserName(string userName)
        {
            JArray jsonArray = new JArray();
            string userFullName = string.Empty;

            var userDetails = (from user in db.Users
                               where user.UserName == userName
                               select user).FirstOrDefault();

            if (userDetails != null)
                userFullName = userDetails.FirstName + ' ' + userDetails.LastName;

            return userFullName;
        }

        //public AppUserViewModel GetUserViewModel(string userName)
        //{
        //    if (!string.IsNullOrEmpty(userName))
        //    {


        //        using (var db = new ApplicationDbContext())
        //        {
        //            var vm = db.Users.Where(x=> x.UserName == userName || x.Email == userName).ToList().Select(x => new AppUserViewModel
        //            {
        //                Email = x.Email,
        //                Id = x.Id,
        //                Name = $"{x.FirstName} {x.LastName}"
        //            }).FirstOrDefault();

        //            if (vm != null)
        //            {
        //                return vm;
        //            }


        //        }
        //    }

        //    return new AppUserViewModel
        //    {
        //        Id = 6,
        //        Name = "User not found"
        //    };


        //}

        //#region  Delete User

        //public bool DeleteUser(int userId)
        //{
        //    if (userId > 0)
        //    {
        //        try
        //        {
        //            using (var db = new HAVA_DBModelEntities())
        //            {


        //                var additionalData = (from data in db.UserAdditionalDatas
        //                                      where data.UserId == userId
        //                                      select data).FirstOrDefault();
        //                if (additionalData != null)
        //                {
        //                    var entry = db.Entry(additionalData);
        //                    entry.State = System.Data.Entity.EntityState.Deleted;
        //                    db.SaveChanges();
        //                }


        //            }
        //            using (var db = new ApplicationDbContext())
        //            {


        //                var userData = (from data in db.Users
        //                                where data.Id == userId
        //                                select data).FirstOrDefault();
        //                var entry = db.Entry(userData);
        //                entry.State = System.Data.Entity.EntityState.Deleted;
        //                db.SaveChanges();


        //            }
        //            return true;
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //        }

        //    }
        //    else
        //    {
        //        return false;
        //    }



        //}


        //public int GetEmployeeNoByUserId(int userId)
        //{
        //    JArray jsonArray = new JArray();
        //    int employeeNo   = 0;
        //    var userDetails  = (from userData in gb_db.UserAdditionalDatas 
        //                       where userData.UserId == userId
        //                       select userData).FirstOrDefault();

        //    if (userDetails != null && userDetails.EmployeeNo != null)
        //        employeeNo = userDetails.EmployeeNo.Value;

        //    return employeeNo;
        //}


        //public bool SendEmailForForgetPassword(string  userName , out int errorType)
        //{
        //    try
        //    {
        //        errorType                = -1;
        //        AppUserViewModel appUser = GetUserViewModel(userName);

        //        if ( ! string.IsNullOrEmpty(appUser.Email))
        //        {
        //            //Generate Email

        //            //MailerService service = new MailerService();
        //            //service.SendMail(ReadSetting("FromEmail"), appUser.Email, "Reset Password", GetEmailBodyForPasswordReset(appUser));
        //            return true;
        //        }
        //        else
        //        {     
        //            errorType = 1;
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        errorType = 2;
        //        return false;
        //    }

        //}


        //#region Read Setting
        static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                var result = appSettings[key] ?? "Not Found";
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                return null;
            }
        }
       

        //#region  Get Email Body For Password Reset
        private string GetEmailBodyForPasswordReset(AppUserViewModel appuser)
        {
            string body        = string.Empty;
            //string encryptedId = Helper.EncryptText(appuser.Id.Value.ToString());
            //string link        = ReadSetting("WebUrl") + "/Account/ResetUserPassword#/?UserName="+ encryptedId;
            //body               = string.Format(NotificationTemplates.ResetPasswordEmail, appuser.Name, link);

            return body;
        }

       

    }
}