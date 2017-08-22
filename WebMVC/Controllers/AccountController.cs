using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Newtonsoft.Json.Linq;
using WebMVC.ModelViews;
using WebMVC.Models;
using WebMVC.Common;
using HavaBusiness;

namespace WebMVC.Controllers
{
    [System.Web.Http.AllowAnonymous]
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private const string LocalLoginProvider = "Local";

        #region repository db context

        private HAVA_DBModelEntities context;

        private HAVA_DBModelEntities ObjContext
        {
            get
            {
                if (context == null)
                    context = new HAVA_DBModelEntities();
                return context;
            }
        }
        #endregion db context

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
                                 ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (_userManager != null))
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        // GET api/Account/ExternalLogin
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogin", Name = "ExternalLogin")]

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        //[System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            var descriptions = Authentication.GetExternalAuthenticationTypes();
            var logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (var description in descriptions)
            {
                var login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        [System.Web.Http.Route("LoggedIn")]
        public async Task<JsonResult> LoggedIn(LoginViewModel loginModel)
        {
            //var messagedata = new object();
            var obj = new JsonResult();

            try
            {
                if (ModelState.IsValid)
                {
                    // This doen't count login failures towards lockout only two factor authentication
                    // To enable password failures to trigger lockout, change to shouldLockout: true
                    var result = SignInManager.PasswordSignIn(loginModel.UserName, loginModel.Password,
                        loginModel.RememberMe, false);
                }
            }
            catch (Exception ex)
            {
            }
            return obj;
        }

        // POST api/Account/Logout
        [System.Web.Http.Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/Register
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            var result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RegisterExternal
        [System.Web.Http.OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [System.Web.Http.Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            var result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }


        [System.Web.Http.Route("ResetPassword")]
        [System.Web.Http.HttpPost]
        public JObject ResetPassword(PasswordResetViewModel model)
        {
            var obj = new JObject();
            var errorType = -1;
            try
            {
                // Decrypt userId
                var userId = int.Parse(Helper.DecryptText(model.UserName.Replace("%2F", "/") + "=="));
                var userRepository = new UserRepository();
                var token = UserManager.GeneratePasswordResetToken(userId);
                var success = UserManager.ResetPassword(userId, token, model.Password);
                if (success.Succeeded)
                {
                    obj.Add("isSuccess", true);
                    obj.Add("errorType", errorType);
                }
                else
                {
                    obj.Add("isSuccess", false);
                    obj.Add("errorType", errorType);
                }

                return obj;
            }
            catch (Exception ex)
            {
                obj.Add("isSuccess", false);
                obj.Add("errorType", errorType);
                return obj;
            }
        }

        [System.Web.Http.Route("ChangePassword")]
        [System.Web.Http.HttpPost]
        public JObject ChangePassword(ChangePasswordViewModel model)
        {
            var obj = new JObject();
            var errorType = -1;
            try
            {

                var success = UserManager.ChangePassword(model.UserId, model.CurrentPassword, model.NewPassword);
                if (success.Succeeded)
                {
                    obj.Add("isSuccess", true);
                    obj.Add("errorType", errorType);
                }
                else
                {
                    obj.Add("isSuccess", false);
                    obj.Add("errorType", errorType);
                }

                return obj;
            }
            catch (Exception ex)
            {
                obj.Add("isSuccess", false);
                obj.Add("errorType", errorType);
                return obj;
            }
        }

        [System.Web.Http.Route("SendEmailForForgetPassword")]
        [System.Web.Http.HttpGet]
        public JObject SendEmailForForgetPassword(string userName)
        {
            var obj = new JObject();
            var errorType = -1;
            try
            {
                var userRepository = new UserRepository();
                bool success = true;

                //var success = userRepository.SendEmailForForgetPassword(userName, out errorType);
                if (success)
                {
                    obj.Add("isSuccess", true);
                    obj.Add("errorType", errorType);
                }
                else
                {
                    obj.Add("isSuccess", false);
                    obj.Add("errorType", errorType);
                }

                return obj;
            }
            catch (Exception ex)
            {
                obj.Add("isSuccess", false);
                obj.Add("errorType", errorType);
                return obj;
            }
        }

        [System.Web.Http.Route("CreateAppUser")]
        [System.Web.Http.HttpPost]
        public JObject CreateWebUsers(ApplicationUser appUser)
        {
            JObject returnObj = new JObject();
            var result = UserManager.CreateAsync(appUser, appUser.Email).Result;

            returnObj.Add("data", this.ObjContext.Users.Where(u => u.UserName.ToLower() == appUser.UserName.ToLower()).FirstOrDefault<User>().Id);
            return returnObj;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if ((providerKeyClaim == null) || string.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || string.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static readonly RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                var strengthInBytes = strengthInBits / bitsPerByte;

                var data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }
    }
}