using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using TaskList.Api.Filters;
using TaskList.Api.Models;
using TaskList.Model.Models;
using System.Collections.Generic;

namespace TaskList.Api.Controllers
{
    //[Authorize, RoutePrefix("api/Account")]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";

        public AccountController()
            : this(Startup.UserManagerFactory(), Startup.OAuthOptions.AccessTokenFormat)
        {
        }

        public AccountController(UserManager<TaskUser> userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
            //IdentityResult result = null;
            //if (userManager.FindByName("dbeech") == null)
            //    result = userManager.Create(new MisconnexUser { 
            //        UserType = Model.Enums.UserType.Management,
            //        Email = "dbeech@e9ine.com",
            //        UserName = "dbeech"
            //    }, "DavidBeech23");
            //var x = result.Succeeded;
        }

        public UserManager<TaskUser> UserManager { get; private set; }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public TaskUser GetUserInfo()
        {
            return UserManager.FindById(User.Identity.GetUserId());
        }

        [Queryable, Route("AllUsers")]
        public IHttpActionResult GetUsers()
        {
            return Ok(UserManager.Users);
        }

        [Queryable, Route("getallusers")]
        public IHttpActionResult GetAllUsers()
        {
            //var users = UserManager.Users.Where(u => u.UserName.Contains(userID)).Select(u => new { u.UserName }).ToList();
            var users = UserManager.Users.Select(u => new { u.UserName }).ToList();
            return Ok(users);
        }

        [ValidateModel, HttpPost, Route("Create")]
        public IHttpActionResult CreateUser(NewUserBindingModel model)
        {            
            var user = new TaskUser { 
                    UserName = model.UserName,
                    Email = model.Email,
                    LockoutEndDateUtc = DateTime.Now
                };  
            var result = UserManager.Create(user, model.Password);

            if (result.Succeeded)
                return Ok(user);
            else
                return InternalServerError(new Exception(String.Join(", ", result.Errors)));
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/ChangePassword
        [HttpPost, Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [HttpPost, Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var targetUser = UserManager.FindById(model.Id);

            if (targetUser == null)
                return NotFound();

            if (currentUser == null)
                return BadRequest("Not logged in");

            /*switch (currentUser.UserType)
            {
                case UserType.Management:
                    break;
                case UserType.Airline:
                    return BadRequest("Only Admin or Management can set password of users");
                case UserType.AirlineAdmin:
                    if (!(targetUser.AirlineId.HasValue && targetUser.AirlineId.Value == currentUser.AirlineId))
                        return BadRequest("Can only set password of users to own airline");
                    break;
                case UserType.Hotel:
                    if (!(targetUser.HotelId.HasValue && targetUser.HotelId.Value == currentUser.HotelId))
                        return BadRequest("Can only set password of users to own hotel");
                    break;
                case UserType.Handler:
                    return BadRequest("Only Admin or Management can set password of users");
                case UserType.HandlerAdmin:
                    if (!(targetUser.HandlerId.HasValue && targetUser.HandlerId.Value == currentUser.HandlerId))
                        return BadRequest("Can only set password of users to own handler");
                    break;
                default:
                    return BadRequest("Current user must be logged in to set password of users");
            }*/

            UserManager.RemovePassword(model.Id);
            IdentityResult result = await UserManager.AddPasswordAsync(model.Id, model.NewPassword);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        [HttpPost, Route("LockUser")]
        public IHttpActionResult LockUser(string id)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var targetUser = UserManager.FindById(id);

            if (currentUser == null)
                return BadRequest("Current user must be logged in to lock users");

            if (targetUser == null)
                return NotFound();

            /*switch (currentUser.UserType)
            {
                case UserType.Management:
                    break;
                case UserType.Airline:

                    return BadRequest("Only Admin or Management can lock users");
                case UserType.AirlineAdmin:
                    if (!(targetUser.AirlineId.HasValue && targetUser.AirlineId.Value == currentUser.AirlineId))
                        return BadRequest("Can only lock users to own airline");
                    break;
                case UserType.Hotel:
                    if (!(targetUser.HotelId.HasValue && targetUser.HotelId.Value == currentUser.HotelId))
                        return BadRequest("Can only lock users to own hotel");
                    break;
                case UserType.Handler:
                    return BadRequest("Only Admin or Management can lock users");
                case UserType.HandlerAdmin:
                    if (!(targetUser.HandlerId.HasValue && targetUser.HandlerId.Value == currentUser.HandlerId))
                        return BadRequest("Can only lock users to own handler");
                    break;
                default:
                    return BadRequest("Current user must be logged in to lock users");
            }*/

            UserManager.SetLockoutEnabled(id, true);
            UserManager.SetLockoutEndDate(id, DateTimeOffset.UtcNow.AddYears(10));
            return Ok(new { LockoutEnabled = true });
        }

        [HttpPost, Route("UnlockUser")]
        public IHttpActionResult UnlockUser(string id)
        {
            var currentUser = UserManager.FindById(User.Identity.GetUserId());
            var targetUser = UserManager.FindById(id);

            if (currentUser == null)
                return BadRequest("Current user must be logged in to lock users");

            if (targetUser == null)
                return NotFound();

            /*switch (currentUser.UserType)
            {
                case UserType.Management:
                    break;
                case UserType.Airline:

                    return BadRequest("Only Admin or Management can lock users");
                case UserType.AirlineAdmin:
                    if (!(targetUser.AirlineId.HasValue && targetUser.AirlineId.Value == currentUser.AirlineId))
                        return BadRequest("Can only lock users to own airline");
                    break;
                case UserType.Hotel:
                    if (!(targetUser.HotelId.HasValue && targetUser.HotelId.Value == currentUser.HotelId))
                        return BadRequest("Can only lock users to own hotel");
                    break;
                case UserType.Handler:
                    return BadRequest("Only Admin or Management can lock users");
                case UserType.HandlerAdmin:
                    if (!(targetUser.HandlerId.HasValue && targetUser.HandlerId.Value == currentUser.HandlerId))
                        return BadRequest("Can only lock users to own handler");
                    break;
                default:
                    return BadRequest("Current user must be logged in to lock users");
            }*/

            UserManager.SetLockoutEnabled(id, false);
            UserManager.SetLockoutEndDate(id, DateTimeOffset.UtcNow);
            return Ok(new { LockoutEnabled = false });
        }

        [HttpGet, Route("Unique")]
        public IHttpActionResult Unique(string username)
        {
            var user = UserManager.FindByName(username);

            if (user == null)
            {
                return Ok(new { result = true });
            }
            else
            {
                return Ok(new { result = false });
            }
        }

        [HttpPost, Route("SetEmail")]
        public IHttpActionResult SetEmail(string id, string email)
        {
            var user = UserManager.FindById(id);

            user.Email = email;
            UserManager.Update(user);
            return Ok(user);
        }

        [HttpPut]
        public IHttpActionResult Put(TaskUser update)
        {
            var user = UserManager.FindById(update.Id);
            if (user == null)
                return NotFound();

            //user.UserType = update.UserType;
            user.Email = update.Email;

            UserManager.Update(user);
            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
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
                    foreach (string error in result.Errors)
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

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
