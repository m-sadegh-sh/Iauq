using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Iauq.Core.Domain;
using Iauq.Core.Utilities;
using Iauq.Data.Services;

namespace Iauq.Web.Utilities
{
    public class WebHelper : IWebHelper
    {
        private static Language _currentLanguage;
        private static User _currentUser;

        private readonly ILanguageService _languageService;
        private readonly IUserService _userService;

        public WebHelper(ILanguageService languageService, IUserService userService)
        {
            _languageService = languageService;
            _userService = userService;
        }

        #region IWebHelper Members

        public Language CurrentLanguage
        {
            get
            {
                if (_currentLanguage != null)
                    return _currentLanguage;

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentUICulture;

                string isoCode = cultureInfo.TwoLetterISOLanguageName;

                _currentLanguage = _languageService.GetLanguageByIsoCode(isoCode);

                if (_currentLanguage == null)
                    _currentLanguage = _languageService.GetFallbackLanguage();

                return _currentLanguage;
            }
        }

        public User GetCurrentUser(HttpContextBase httpContext)
        {
            return GetCurrentUser(httpContext.ApplicationInstance.Context);
        }

        public User GetCurrentUser(HttpContext httpContext)
        {
            IPrincipal userPrincipal = httpContext.User;

            if (userPrincipal == null || !userPrincipal.Identity.IsAuthenticated)
                return null;

            string userName = userPrincipal.Identity.Name;

            _currentUser = _userService.GetUserByUserName(userName);

            return _currentUser;
        }

        public bool IsInRoleWithRoles(params string[] roles)
        {
            return IsInRole(GetCurrentUser(HttpContext.Current), roles);
        }

        public bool IsInRole(string userName, params string[] roles)
        {
            return IsInRole(_userService.GetUserByUserName(userName), roles);
        }

        public bool IsInRole(User user, params string[] roles)
        {
            if (user == null)
                return false;

            if (roles == null || roles.Length == 0)
                return true;

            bool isInRole = user.Roles.Any(ur => roles.Any(r => string.CompareOrdinal(ur.Name, r) == 0));

            return isInRole;
        }

        public string MapUrl(string path)
        {
            string appPath = HttpContext.Current.Server.MapPath("~");

            string url = string.Format("/{0}", path.Replace(appPath, "").Replace("\\", "/")).Replace("//", "/");

            return url;
        }

        #endregion

        public string GetIpAddress()
        {
            string ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            return ipAddress;
        }
    }
}