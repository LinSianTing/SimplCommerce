using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace SimplCommerce.Module.Core.Extensions
{
    public class WorkContext : IWorkContext
    {
        private const string SystemAppSectionName = "SystemApp";
        private const string UserGuidCookiesName = "SimplUserGuid";
        private const long GuestRoleId = 3;

        private SystemApp _currentSystemApp;
        private User _currentUser;
        private UserManager<User> _userManager;
        private HttpContext _httpContext;
        private IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public WorkContext(UserManager<User> userManager,
                           IHttpContextAccessor contextAccessor,
                           IRepository<User> userRepository,
                           IConfiguration configuration)
        {
            _userManager = userManager;
            _httpContext = contextAccessor.HttpContext;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Add By Eric Lin
        /// </summary>
        /// <returns></returns>
        public SystemApp GetCurrentSystemAppSync()
        {
            if (_currentSystemApp != null)
            {
                return _currentSystemApp;
            }

            int id = 0;
            var idObject = _configuration.GetSection(@SystemAppSectionName + ":" + "Id").Value;
            if (!int.TryParse(idObject, out id))
            {
                return null;
            }
            string name = _configuration.GetSection(@SystemAppSectionName + ":" + "Name").Value;
            string description = _configuration.GetSection(@SystemAppSectionName + ":" + "Description").Value;

            bool enableSystemIdCross = false;
            var enableSystemIdCrossObject = _configuration.GetSection(@SystemAppSectionName + ":" + "EnableSystemIdCross").Value;
            if (!bool.TryParse(enableSystemIdCrossObject, out enableSystemIdCross))
            {

            }

            List<int> IdCrossList = null;
            string systemIdCrossListstring = _configuration.GetSection(@SystemAppSectionName + ":" + "SystemIdCross").Value;
            if (!string.IsNullOrEmpty(systemIdCrossListstring))
            {
                string[] systemIdCrossArray = systemIdCrossListstring.Split(',');

                if (systemIdCrossArray.Length != 0)
                {
                    IdCrossList = new List<int>();
                    for (int i = 0; i <= systemIdCrossArray.Length - 1; i++)
                    {
                        Int16 idCross = 0;
                        var idCrossObject = systemIdCrossArray[i];
                        if (Int16.TryParse(idCrossObject, out idCross))
                        {
                            IdCrossList.Add(idCross);
                        }
                    }
                }
            }

            var versionStageObject = _configuration.GetSection(@SystemAppSectionName + ":" + "VersionStage").Value;
            VersionStageType versionStage = VersionStageType.Develop;
            if (!VersionStageType.TryParse(versionStageObject, out versionStage))
            {

            }

            _currentSystemApp = new SystemApp(id, name, description, enableSystemIdCross, IdCrossList, versionStage);

            return _currentSystemApp;
        }

        /// <summary>
        /// Add By Eric Lin
        /// </summary>
        /// <returns></returns>
        public async Task<SystemApp> GetCurrentSystemApp()
        {
            if (_currentSystemApp != null)
            {
                return _currentSystemApp;
            }

            int id = 0;
            var idObject = _configuration.GetSection(@SystemAppSectionName + ":" + "Id").Value;
            if (!int.TryParse(idObject, out id))
            {
                return null;
            }
            string name = _configuration.GetSection(@SystemAppSectionName + ":" + "Name").Value;
            string description = _configuration.GetSection(@SystemAppSectionName + ":" + "Description").Value;

            bool enableSystemIdCross = false;
            var enableSystemIdCrossObject = _configuration.GetSection(@SystemAppSectionName + ":" + "EnableSystemIdCross").Value;
            if (!bool.TryParse(enableSystemIdCrossObject, out enableSystemIdCross))
            {

            }

            List<int> IdCrossList = null;
            string systemIdCrossListstring = _configuration.GetSection(@SystemAppSectionName + ":" + "SystemIdCross").Value;
            if (!string.IsNullOrEmpty(systemIdCrossListstring))
            {
                string[] systemIdCrossArray = systemIdCrossListstring.Split(',');

                if (systemIdCrossArray.Length != 0)
                {
                    IdCrossList = new List<int>();
                    for (int i = 0; i <= systemIdCrossArray.Length - 1; i++)
                    {
                        Int16 idCross = 0;
                        var idCrossObject = systemIdCrossArray[i];
                        if (Int16.TryParse(idCrossObject, out idCross))
                        {
                            IdCrossList.Add(idCross);
                        }
                    }
                }
            }

            var versionStageObject = _configuration.GetSection(@SystemAppSectionName + ":" + "VersionStage").Value;
            VersionStageType versionStage = VersionStageType.Develop;
            if (!VersionStageType.TryParse(versionStageObject, out versionStage))
            {

            }

            _currentSystemApp = new SystemApp(id, name, description, enableSystemIdCross, IdCrossList, versionStage);

            return _currentSystemApp;
        }

        public async Task<User> GetCurrentUser()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            var contextUser = _httpContext.User;
            _currentUser = await _userManager.GetUserAsync(contextUser);

            if (_currentUser != null)
            {
                return _currentUser;
            }

            var userGuid = GetUserGuidFromCookies();
            if (userGuid.HasValue)
            {
                _currentUser = _userRepository.Query().Include(x => x.Roles).FirstOrDefault(x => x.UserGuid == userGuid);
            }

            if (_currentUser != null && _currentUser.Roles.Count == 1 && _currentUser.Roles.First().RoleId == GuestRoleId)
            {
                return _currentUser;
            }

            userGuid = Guid.NewGuid();
            var dummyEmail = string.Format("{0}@guest.simplcommerce.com", userGuid);
            _currentUser = new User
            {
                FullName = "Guest",
                UserGuid = userGuid.Value,
                Email = dummyEmail,
                UserName = dummyEmail,
                Culture = _configuration.GetValue<string>("Global.DefaultCultureUI") ?? GlobalConfiguration.DefaultCulture
            };
            var abc = await _userManager.CreateAsync(_currentUser, "123456");
            await _userManager.AddToRoleAsync(_currentUser, "guest");
            SetUserGuidCookies(_currentUser.UserGuid);
            return _currentUser;
        }

        private Guid? GetUserGuidFromCookies()
        {
            if (_httpContext.Request.Cookies.ContainsKey(UserGuidCookiesName))
            {
                return Guid.Parse(_httpContext.Request.Cookies[UserGuidCookiesName]);
            }

            return null;
        }

        private void SetUserGuidCookies(Guid userGuid)
        {
            _httpContext.Response.Cookies.Append(UserGuidCookiesName, _currentUser.UserGuid.ToString(), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddYears(5),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            });
        }
    }
}
