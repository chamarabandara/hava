using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Threading.Tasks;
using HavaBusiness;
using WebMVC.ModelViews;

namespace WebMVC.Models
{
    public class AuthRepository : IDisposable
    {
        //#region Initializations
        private HAVA_DBModelEntities context;
        private ApplicationDbContext ctx ;
        private ApplicationUserManager _userManager;
       
        private UserManager<ModelViews.AspNetUser> userManager;
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
        

        private HAVA_DBModelEntities ObjContext
        {
            get
            {
                if (context == null)
                    context = new HAVA_DBModelEntities();
                return context;
            }
        }
       
        
        public AuthRepository()
        {
            ctx = new ApplicationDbContext();
            userManager = new UserManager<ModelViews.AspNetUser>(new UserStore<ModelViews.AspNetUser>(ObjContext));
        }
       

        public void Dispose()
        {
            ctx.Dispose();
            userManager.Dispose();
        }
       

        public async Task<List<string>> GetUserPermissionsAsync(string userName)
        {
            ApplicationUser appuser     = UserManager.FindByName(userName);
            var userRoles               = await UserManager.GetRolesAsync(appuser.Id);
            List<string> lstPermissions = userRoles.ToList();
           
            return lstPermissions.Distinct().ToList();
        }

        
        public async Task<ApplicationUser> GetAspNetUserAsync(string userName, string password)
        {

            ApplicationUser appuser = UserManager.FindByName(userName);
           
            bool isLoginSuccess     =  UserManager.CheckPassword(appuser, password);
         
           
            return appuser;
        }

        
        public bool CheckGetAspNetUser(string userName, string password)
        {

            ApplicationUser appuser = UserManager.FindByName(userName);

            bool isLoginSuccess = UserManager.CheckPassword(appuser, password);


            return isLoginSuccess;
        }

        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        
        public AspNetClient GetAspNetClient(string clientId)
        {
            AspNetClient client = ctx.Clients.Find(clientId);
            return client;
        }

       
        public async Task<bool> AddRefreshToken(AspNetRefreshToken token)
        {
            var existingToken = ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).FirstOrDefault();

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }

            ctx.RefreshTokens.Add(token);

            return await ctx.SaveChangesAsync() > 0;
        }

        
        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                ctx.RefreshTokens.Remove(refreshToken);
                return await ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        
        public async Task<bool> RemoveRefreshToken(AspNetRefreshToken refreshToken)
        {
            ctx.RefreshTokens.Remove(refreshToken);
            return await ctx.SaveChangesAsync() > 0;
        }

        
        public async Task<AspNetRefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<AspNetRefreshToken> GetAllRefreshTokens()
        {
            return ctx.RefreshTokens.ToList();
        }

       
    }
}