using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Areas.Catalog.ViewModels;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
//using SimplCommerce.Module.NongMinGo.Areas.Catalog.ViewModels;
//using SimplCommerce.Module.NongMinGo.Models;
//using SimplCommerce.Module.NongMinGo.Services;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.NongMinGo.Areas.NongMinGo.ViewModels;

namespace SimplCommerce.Module.NongMinGo.Areas.NongMinGo.Controllers
{
    [Area("NongMinGo")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("shopstore")]
    public class ShopStoreController : Controller
    {
        private readonly ILogger _logger;
        private readonly IRepository<User> _userRepository;
        private readonly IWidgetInstanceService _widgetInstanceService;
        private readonly IWorkContext _workContext;
        private readonly IConfiguration _config;

        public ShopStoreController(ILoggerFactory factory, IRepository<User> userRepository, IWidgetInstanceService widgetInstanceService, IWorkContext workContext, IConfiguration config)
        {
            _logger = factory.CreateLogger("Unhandled Error");
            _widgetInstanceService = widgetInstanceService;
            _userRepository = userRepository;
            _workContext = workContext;
            _config = config;
        }

        public async Task<IActionResult> ShopStoreDetail()
        {
            return await ExecuteShopStoreDetail(null);
        }

        [Route("{id}")]
        [Route("/ui/{id}")]
        public async Task<IActionResult> ShopStoreDetail(long id)
        {
            return await ExecuteShopStoreDetail(id);
        }

        [Route("/un/{name}")]
        public async Task<IActionResult> ShopStoreDetail(string name)
        {
            User user = null;

            if (!string.IsNullOrEmpty(name))
            {
                user = await _userRepository.Query()
                .Include(x => x.Roles)
                .Include(x => x.CustomerGroups)
                .FirstOrDefaultAsync(x => x.FullName == name);
            }

            if(user == null)
            {
                return NotFound();
            }

            return await ExecuteShopStoreDetail(user.Id);
        }

        private async Task<IActionResult> ExecuteShopStoreDetail(long? id)
        {
            var user = await _workContext.GetCurrentUser();

            // CodeNote : 首先進入controller，接下來指定要運行的ViewModel，最後透過 Return View(); 把 前後端整合。
            var model = new ShopStoreViewModel();

            model.WidgetInstances = _widgetInstanceService.GetPublished()
                .OrderBy(x => x.DisplayOrder)
                .Select(x => new WidgetInstanceViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    ViewComponentName = x.Widget.ViewComponentName,
                    WidgetId = x.WidgetId,
                    WidgetZoneId = x.WidgetZoneId,
                    Data = x.Data,
                    HtmlData = x.HtmlData
                }).ToList();

            if (id != null)
            {
                model.ShopStoreId = id;

                foreach (WidgetInstanceViewModel viewModel in model.WidgetInstances)
                {
                    if (viewModel.Id == 9)
                    {
                        var setting = JsonConvert.DeserializeObject<ProductWidgetSetting>(viewModel.Data);
                        setting.ShopStoreId = model.ShopStoreId;
                        viewModel.Data = JsonConvert.SerializeObject(setting);
                    }
                }
            }

            return View(model);
        }
    }
}
