using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;
using SimplCommerce.Module.Core.Extensions;
//using SimplCommerce.Module.NongMinGo.Areas.Catalog.ViewModels;
//using SimplCommerce.Module.NongMinGo.Models;
//using SimplCommerce.Module.NongMinGo.Services;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.NongMinGo.Areas.NongMinGo.ViewModels;

namespace SimplCommerce.Module.NongMinGo.Areas.NongMinGo.Controllers
{
    [Area("NongMinGo")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("community")]
    public class CommunityController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly IWidgetInstanceService _widgetInstanceService;

        public CommunityController(ILoggerFactory factory, IWidgetInstanceService widgetInstanceService, IWorkContext workContext, IConfiguration config)
        {
            _workContext = workContext;
            _config = config;
            _logger = factory.CreateLogger("Unhandled Error");
            _widgetInstanceService = widgetInstanceService;
        }

        public async Task<IActionResult> CommunityDetail(long id)
        {
            var model = new CommunityViewModel();

            var user = await _workContext.GetCurrentUser();

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

            return View(model);
        }
    }
}
