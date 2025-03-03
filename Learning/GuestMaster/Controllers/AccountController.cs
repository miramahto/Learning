using GuestMaster.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NuGet.Protocol.Plugins;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Utility;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace GuestMaster.Controllers
{
  

    public class AccountController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly IConfiguration _configuration;
        public AccountController(EMSDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var jsonContent = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var baseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");

            using (var httpClient = new HttpClient())
            {
                //var response = await httpClient.PostAsync("http://localhost:5019/account/login", content);
                var response = await httpClient.PostAsync($"{baseUrl}/account/login", content);

                string userRole = string.Empty;
                string guid = string.Empty;
                string appName= string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync(); // Assuming the token is returned in the response content
                    var userToken = JsonSerializer.Deserialize<userToken>(tokenResponse);
                    string token = userToken.token; // Convert user role to string variable
                                                    
                    HttpContext.Session.SetString(SessionKeys.UserToken, token);// Save token to session
                    HttpContext.Session.SetString(SessionKeys.UserEmail, model.Email);// Save User Email to session
                    using (var httpClientRole = new HttpClient())
                    {
                        httpClientRole.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        //var Roleresponse = await httpClientRole.GetAsync($"http://localhost:5019/account/getUserDetails?email={model.Email}&AppTypeName=GEM");
                        var Roleresponse = await httpClientRole.GetAsync($"{baseUrl}/account/getUserDetails?email={model.Email}&AppTypeName=GEM");

                        if (Roleresponse.IsSuccessStatusCode)
                        {

                            var responseContent = await Roleresponse.Content.ReadAsStringAsync();
                            var userRoleObject = JsonSerializer.Deserialize<userDetails>(responseContent);
                            userRole = userRoleObject.role; // Convert user role to string variable
                            guid = userRoleObject.userapp.guid;
                            appName = userRoleObject.userapp.appName;
                            HttpContext.Session.SetString(SessionKeys.UserRole, userRole);
                            HttpContext.Session.SetString(SessionKeys.UserGUID, guid);
                            HttpContext.Session.SetString(SessionKeys.UserAppName, appName);
                            // LINQ query to get AppMaster by AppsGUID
                            if (!string.IsNullOrEmpty(guid))
                            {
                                var appMaster = _context.AppMasters
                                .FirstOrDefault(a => a.AppsGUID == Guid.Parse(guid));
                                if (appMaster != null)
                                {
                                   
                                    HttpContext.Session.SetString(SessionKeys.isUserRoleAppMapped, "TRUE");
                                }
                                else
                                {
                                    HttpContext.Session.SetString(SessionKeys.isUserRoleAppMapped, "FALSE");
                                }

                            }
                          


                        }
                        else
                        {
                            HttpContext.Session.SetString(SessionKeys.isUserRoleAppMapped,"FALSE");
                            
                           
                        }


                    }



                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, model.Email),
                            new Claim(ClaimTypes.Role, userRole)

                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties);

                        return RedirectToAction("Index", "Home");
                    
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
        private class userDetails
        {
            public string role { get; set; }
            public UserAppDetails userapp { get; set; }
        }
        private class UserAppDetails
        {
            public string guid { get; set; }
            public string appName { get; set; }

        }
        private class userToken
        {
            public string token { get; set; }
        }
    }

}
