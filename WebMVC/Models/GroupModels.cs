using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using WebMVC.Models;
using WebMVC.ModelViews;
using HavaBusiness;

namespace WebMVC.Models
{
    public class GroupModels
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }
        public List<ApplicationGroup> Groups()
        {
            var groups = _db.ApplicationGroups.ToList();
            return groups;
        }

        public ApplicationGroup View(int id)
        {
            var group = _db.ApplicationGroups.Find(id);
            return group;
        }

        public int Add(ApplicationGroup group)
        {
            _db.ApplicationGroups.Add(group);
            return _db.SaveChanges();
        }

        public int Edit(ApplicationGroup group)
        {
            _db.Entry(group).State = EntityState.Modified;
            return _db.SaveChanges();
        }

        public int Delete(int groupId)
        {
            ApplicationGroup group = _db.ApplicationGroups.Find(groupId);

            // Clear the roles from the group:
            ClearGroupRoles(groupId, group);
            _db.ApplicationGroups.Remove(group);
            try
            {
                return _db.SaveChanges();
            }
            catch (Exception ex) { return 1; }
        }

        private void ClearGroupRoles(int groupId, ApplicationGroup group)
        {
            IList<ApplicationUser> groupUsers = _db.Users.Where(u => u.Groups.Any(g => g.GroupId == group.Id)).ToList();

            foreach (ApplicationRoleGroup role in group.Roles)
            {
                int currentRoleId = role.RoleId;
                foreach (ApplicationUser user in groupUsers)
                {
                    // Is the user a member of any other groups with this role?
                    int groupsWithRole = user.Groups.Count(g => g.Group.Roles.Any(r => r.RoleId == currentRoleId));

                    // This will be 1 if the current group is the only one:
                    if (groupsWithRole == 1)
                    {
                        this.RemoveFromRole(user.Id, role.Role.Name);
                    }
                }
            }

            try
            {
                group.Roles.Clear();
                _db.SaveChanges();
            }
            catch (Exception ex)
            { }
        }

        private void RemoveFromRole(int userId, string roleName)
        {
            UserManager.RemoveFromRole(userId, roleName);
        }

        public void AddRolesToGroup(SelectGroupRolesViewModel groupRoles, string[] selectedRole)
        {
            var group = _db.ApplicationGroups.Find(groupRoles.GroupId);
            this.ClearGroupRoles(groupRoles.GroupId, group);

            // Add each selected role to this group:
            foreach (var role in selectedRole)
            {
                this.AddRoleToGroup(group.Id, role, group);
            }
        }

        public void AddRoleToGroup(int groupId, string roleName, ApplicationGroup group)
        {
            ApplicationRole role = _db.Roles.First(r => r.Name == roleName);

            var newgroupRole = new ApplicationRoleGroup
            {
                GroupId = group.Id,
                Group = group,
                RoleId = role.Id,
                Role = role
            };

            // make sure the groupRole is not already present
            if (!group.Roles.Contains(newgroupRole))
            {
                group.Roles.Add(newgroupRole);
                _db.SaveChanges();
            }

            // Add all of the users in this group to the new role:
            IQueryable<ApplicationUser> groupUsers = _db.Users.Where(u => u.Groups.Any(g => g.GroupId == group.Id));
            foreach (ApplicationUser user in groupUsers)
            {
                if (!(UserManager.IsInRole(user.Id, roleName)))
                {
                    UserManager.AddToRole(user.Id, roleName);
                }
            }
        }

        public SelectUserGroupsViewModel GetUserGroups(int id)
        {
            var user = _db.Users.Find(id);
            return new SelectUserGroupsViewModel(user);
        }

        public void AddGroupsToUser(SelectUserGroupsViewModel userGroups, string[] selectedGroups)
        {
            try
            {
                var user = _db.Users.FirstOrDefault(u => u.UserName == userGroups.UserName);
                this.ClearUserGroups(user);
                foreach (var groupId in selectedGroups)
                {
                    this.AddUserToGroup(user.Id, int.Parse(groupId));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void ClearUserGroups(ApplicationUser user)
        {
            try
            {
                if (user != null)
                {
                    this.ClearUserRoles(user);
                    user.Groups.Clear();
                    _db.SaveChanges();
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ClearUserRoles(ApplicationUser user)
        {
            try
            {
                foreach (string role in UserManager.GetRoles(user.Id))
                {
                    UserManager.RemoveFromRole(user.Id, role);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void AddUserToGroup(int userId, int groupId)
        {
            ApplicationGroup group = _db.ApplicationGroups.Find(groupId);
            ApplicationUser user = _db.Users.Find(userId);

            var userGroup = new ApplicationUserGroup
            {
                Group = group,
                GroupId = group.Id,
                User = user,
                UserId = user.Id
            };

            foreach (ApplicationRoleGroup role in group.Roles)
            {
                UserManager.AddToRole(userId, role.Role.Name);
            }

            user.Groups.Add(userGroup);
            _db.SaveChanges();
        }

        public UserPermissionsViewModel GetUserPermissions(int userId)
        {
            var user = UserManager.FindById(userId);

            UserPermissionsViewModel userPermissions = new UserPermissionsViewModel();
            userPermissions.UserFullName = String.Format("{0} {1}", user.FirstName, user.LastName);
            userPermissions.Roles = UserManager.GetRoles(userId);
            return userPermissions;
        }

        public List<ApplicationGroupsViewModel> GetAllGroups(GroupListViewModel vm)
        {
            try
            {
                List<ApplicationGroupsViewModel> groups = new List<ApplicationGroupsViewModel>();

                using (var dbContext = new HAVA_DBModelEntities())
                {
                    //var allGroups = dbContext.GetAllGroups();

                    //foreach (var group in allGroups)
                    //{
                    //    ApplicationGroupsViewModel applicationgroup = new ApplicationGroupsViewModel()
                    //    {
                    //        ID = group.ID,
                    //        Name = group.Name,
                    //        Roles = group.Roles
                    //    };

                    //    groups.Add(applicationgroup);
                    //}
                }

                return groups;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public List<ApplicationUserViewModel> GetExistingUsers(int outlet)
        {
            try
            {
                List<ApplicationUserViewModel> users = new List<ApplicationUserViewModel>();

                using (var dbContext = new HAVA_DBModelEntities())
                {
                    //var allUsers = dbContext.GetAllUsers();

                    //foreach (var user in allUsers)
                    //{
                    //    ApplicationUserViewModel applicationUser = new ApplicationUserViewModel()
                    //    {
                    //        Id = user.Id,
                    //        Name = user.Name,
                    //        Email = user.Email,
                    //        PhoneNumber = user.PhoneNumber,
                    //        UserGroups = user.UserGroups,
                    //    };

                    //    users.Add(applicationUser);
                    //}
                }

                return users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}