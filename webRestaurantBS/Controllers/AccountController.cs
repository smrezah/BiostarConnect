using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using webRestaurantBS.Classes;
using webRestaurantBS.Models;

namespace webRestaurantBS.Controllers
{
    public class AccountController : Controller
    {
        private readonly string ldapServer = "LDAP://petrokavian.net";
        private readonly string ldapDomain = "petrokavian.net";
        private readonly string _path = "LDAP://petrokavian.net";
        public string FullNameEn { get; private set; }
        public string DisplayName { get; private set; }
        public string Email { get; private set; }
        public string UserGUID { get; private set; }
        public string ErrorMessage { get; private set; }
        public List<string> OrgUnits { get; private set; }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Username and Password are required.");
                return View();
            }

            LdapAuthentication ldp = new LdapAuthentication(ldapServer);
            bool isAuth = await ldp.IsAuthenticated(ldapDomain,username, password);

            // احراز هویت با LDAP
            bool isAuthenticated = await AuthenticateWithLdapAsync(ldapDomain, username, password);
            

            if (isAuthenticated)
            {
                // دریافت اطلاعات کاربر از LDAP
                var userInfo = await GetUserInfoFromLdapAsync(username);

                if (userInfo != null)
                {
                    // اطلاعات کاربر در لاگ یا هر جایی که نیاز دارید استفاده شود
                    Console.WriteLine($"User Info: {userInfo.DisplayName}, {userInfo.Email}, {userInfo.Phone}, {userInfo.PersonalId}");

                    // ورود به سیستم
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("FullName", userInfo.FullName),
                    new Claim("Email", userInfo.Email),
                    new Claim("Phone", userInfo.Phone),
                    new Claim("PersonalId", userInfo.PersonalId),
                    new Claim("UserGUID", userInfo.UserGUID.ToString())
                };

                    var identity = new ClaimsIdentity(claims, "LDAP");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(principal); // ورود به سیستم

                    return RedirectToAction("Index", "FaceEvent");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        public async Task<bool> AuthenticateWithLdapAsync(string domain, string username, string password)
        {
            bool isAuthenticated = false;

            // انجام عملیات احراز هویت در پس‌زمینه
            await Task.Run(() =>
            {
                try
                {
                    string domainAndUsername = domain + @"\" + username;
                    DirectoryEntry entry = new DirectoryEntry(_path, domainAndUsername, password);

                    // این خط برای اتصال و احراز هویت کاربر استفاده می‌شود
                    object obj = entry.NativeObject;  // اینجا اتصال برقرار می‌شود

                    DirectorySearcher search = new DirectorySearcher(entry)
                    {
                        Filter = "(SAMAccountName=" + username + ")"
                    };

                    search.PropertiesToLoad.Add("cn");
                    SearchResult result = search.FindOne();

                    if (result != null)
                    {
                        // اگر کاربر پیدا شد، اطلاعات را به روز می‌کنیم
                        isAuthenticated = true;
                        FullNameEn = (string)result.Properties["cn"][0];
                        string filterAtt = (string)result.Properties["adspath"][0];
                    }
                    else
                    {
                        // اگر کاربر پیدا نشد، احراز هویت ناموفق است
                        isAuthenticated = false;
                    }
                }
                catch (Exception ex)
                {
                    // مدیریت خطا
                    if (ex.Message.Trim('\n').Trim('\r') == "The user name or password is incorrect.")
                        ErrorMessage = "نام کاربری یا رمز عبور وارد شده صحیح نمی باشد.";
                    else
                        ErrorMessage = ex.Message;

                    isAuthenticated = false;
                }
            });

            return isAuthenticated;
        }

        public async Task<UserInfo> GetUserInfoFromLdapAsync(string username)
        {
            UserInfo userInfo = null;

            // انجام عملیات جستجو LDAP در پس‌زمینه
            await Task.Run(() =>
            {
                try
                {
                    // ساخت اتصال به سرور LDAP با استفاده از نام کاربری و رمز عبور
                    DirectoryEntry entry = new DirectoryEntry(_path);
                    DirectorySearcher search = new DirectorySearcher(entry)
                    {
                        Filter = "(SAMAccountName=" + username + ")"
                    };

                    // مشخص کردن ویژگی‌هایی که باید از نتیجه جستجو بارگذاری شوند
                    search.PropertiesToLoad.Add("displayName");
                    search.PropertiesToLoad.Add("mail");
                    search.PropertiesToLoad.Add("telephoneNumber");
                    search.PropertiesToLoad.Add("employeeID");
                    search.PropertiesToLoad.Add("givenName");
                    search.PropertiesToLoad.Add("objectGUID");

                    SearchResult result = search.FindOne(); // جستجو برای یک کاربر خاص

                    if (result != null)
                    {
                        userInfo = new UserInfo
                        {
                            DisplayName = result.Properties["displayName"]?.Count > 0 ? result.Properties["displayName"][0]?.ToString() : null,
                            Email = result.Properties["mail"]?.Count > 0 ? result.Properties["mail"][0]?.ToString() : null,
                            Phone = result.Properties["telephoneNumber"]?.Count > 0 ? result.Properties["telephoneNumber"][0]?.ToString() : null,
                            PersonalId = result.Properties["employeeID"]?.Count > 0 ? result.Properties["employeeID"][0]?.ToString() : null,
                            FullName = result.Properties["givenName"]?.Count > 0 ? result.Properties["givenName"][0]?.ToString() : null,
                            UserGUID = result.Properties["objectGUID"]?.Count > 0 ? new Guid((byte[])result.Properties["objectGUID"][0]) : Guid.Empty
                        };
                    }
                }
                catch (Exception ex)
                {
                    // مدیریت خطاها
                    Console.WriteLine($"Error retrieving user information: {ex.Message}");
                    userInfo = null;
                }
            });

            return userInfo;
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private bool AuthenticateWithActiveDirectory(string username, string password)
        {
            // پیاده‌سازی احراز هویت در Active Directory با LDAP یا دیگر روش‌ها
            return true; // فقط برای نمونه
        }
    }
}
