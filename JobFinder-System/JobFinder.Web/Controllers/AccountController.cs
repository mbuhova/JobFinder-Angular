namespace JobFinder.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using JobFinder.Data;
    using JobFinder.Models;
    using JobFinder.Web.IdentityConfig;
    using JobFinder.Web.Models.AccountViewModels;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    [Authorize]
    public class AccountController : BaseController
    {
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private ApplicationUserManager userManager;
        private ApplicationSignInManager signInManager;

        public AccountController(IJobFinderData data) : base(data)
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, IJobFinderData data) : base(data)
        {
            this.UserManager = userManager;
            this.SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }

            private set
            {
                this.userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return this.signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }

            private set
            {
                this.signInManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            User current = this.Data.Users.All().Where(u => u.Email == model.Email).FirstOrDefault();
            Company company = current as Company;

            if (company != null && !company.IsApproved)
            {
                this.UserManager.RemoveFromRole(company.Id, "Company");
                this.TempData["NotApproved"] = "You are waiting for approval.";
                return this.RedirectToAction("Login", "Account");
            }

            if (company != null && company.IsApproved)
            {
                this.UserManager.AddToRole(company.Id, "Company");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await this.SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            this.Session["MyMenu"] = null; // new
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.View(model);
            }
        }

        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await this.SignInManager.HasBeenVerifiedAsync())
            {
                return this.View("Error");
            }

            var user = await this.UserManager.FindByIdAsync(await this.SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                var code = await this.UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }

            return this.View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await this.SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError(string.Empty, "Invalid code.");
                    return this.View(model);
            }
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return this.View();
        }

        // mine
        // GET: /Account/RegisterCompany
        [AllowAnonymous]
        public ActionResult RegisterCompany()
        {
            ViewBag.OriginalSectors = this.Data.BusinessSectors.All()
                .Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            // ViewBag.OriginalSectors = this.data.BusinessSectors.All();
            // JobFinder.Models.BusinessSector
            return this.View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCompany(RegisterCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isBulstatValid = this.ValidateBulstat(model.Bulstat);
                bool isNameValid = this.ValidateCompanyName(model.CompanyName);

                if (!isBulstatValid)
                {
                    ViewBag.BulstatErrorMessage = "This bulstat is already taken. Bulstat should be unique.";
                }
                else if (!isNameValid)
                {
                    ViewBag.NameErrorMessage = "This company name is already taken. Company name should be unique.";                   
                }
                else
                {
                    var user = new Company() { UserName = model.Email, Email = model.Email };

                    user.Bulstat = model.Bulstat;
                    user.CompanyName = model.CompanyName;
                    user.PhoneNumber = model.Phone;
                    user.Address = model.Address;
                    user.AboutUs = model.AboutUs;
                    user.WebSite = model.WebSite;

                    var result = await this.UserManager.CreateAsync(user, model.Password);                    
                    if (result.Succeeded)
                    {
                        this.AddBusinessSectors(model.SelectedIds, user.Id);

                        // await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\string.Empty + callbackUrl + "\">here</a>");
                        this.TempData["WaitingApproval"] = "Your registration was successfull. You are waiting for approval. Your profile will be active in 24 hours.";
                        return this.RedirectToAction("RegisterCompany", "Account");
                    }

                    this.AddErrors(result);
                }              
            }

            // If we got this far, something failed, redisplay form
            // ViewBag.OriginalSectors = this.data.BusinessSectors.All();
            ViewBag.OriginalSectors = this.Data.BusinessSectors.All()
                .Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return this.View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Person { UserName = model.Email, Email = model.Email };
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                var result = await this.UserManager.CreateAsync(user, model.Password);

                this.UserManager.AddToRole(user.Id, "Person");
                
                if (result.Succeeded)
                {
                    // await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\string.Empty + callbackUrl + "\">here</a>");
                    this.TempData["Success"] = "Successfull registration. Please log in to view your profile.";
                    return this.RedirectToAction("Register", "Account");
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return this.View("Error");
            }

            var result = await this.UserManager.ConfirmEmailAsync(userId, code);
            return this.View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this.UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await this.UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\string.Empty + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return this.View();
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? this.View("Error") : this.View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await this.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(result);
            return this.View();
        }

        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }

        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await this.SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return this.View("Error");
            }

            var userFactors = await this.UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return this.View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View();
            }

            // Generate the token and send it
            if (!await this.SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return this.View("Error");
            }

            return this.RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await this.AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return this.RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await this.SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return this.View("Lockout");
                case SignInStatus.RequiresVerification:
                    return this.RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await this.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await this.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return this.RedirectToLocal(returnUrl);
                    }
                }

                this.AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.AuthenticationManager.SignOut();
            this.Session["MyMenu"] = null;
            return this.RedirectToAction("SearchOffers", "SearchOffer");
        }

        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return this.View();
        }

        #region Helpers
        private bool ValidateCompanyName(string name)
        {
            var userInDatabase = this.Data.Companies.All().FirstOrDefault(c => c.CompanyName == name);

            if (userInDatabase == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ValidateBulstat(string bulstat)
        {
            var userInDatabase = this.Data.Companies.All().FirstOrDefault(c => c.Bulstat == bulstat);

            if (userInDatabase == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddBusinessSectors(int[] sectorIds, string userId)
        {
            var sectorsToAdd = new List<BusinessSector>();

            foreach (var id in sectorIds)
            {
                var sector = this.Data.BusinessSectors.Find(id);
                sectorsToAdd.Add(sector);
            }

            var user = this.Data.Companies.Find(userId);
            user.BusinessSectors = sectorsToAdd;
            this.Data.SaveChanges();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToAction("SearchOffers", "SearchOffer");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
            }

            public string LoginProvider { get; set; }

            public string RedirectUri { get; set; }

            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = this.RedirectUri };
                if (this.UserId != null)
                {
                    properties.Dictionary[AccountController.XsrfKey] = this.UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, this.LoginProvider);
            }
        }
        #endregion
    }
}