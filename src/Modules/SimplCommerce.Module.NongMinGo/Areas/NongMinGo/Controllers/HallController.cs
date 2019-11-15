using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions;
//using SimplCommerce.Module.NongMinGo.Areas.Catalog.ViewModels;
//using SimplCommerce.Module.NongMinGo.Models;
//using SimplCommerce.Module.NongMinGo.Services;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.NongMinGo.Areas.NongMinGo.Controllers
{
    [Area("NongMinGo")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("hall")]
    public class HallController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly IConfiguration _config;

        public HallController(IWorkContext workContext, IConfiguration config)
        {
            _workContext = workContext;
            _config = config;
        }

        public async Task<IActionResult> HallDetail(long id)
        {
            var user = await _workContext.GetCurrentUser();

            var model = 123;

            return View(model);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]object model)
        {
            if (ModelState.IsValid)
            {
                var user = await _workContext.GetCurrentUser();

                if (!User.IsInRole("admin"))
                {
                    var isCommentsRequireApproval = _config.GetValue<bool>("Catalog.IsCommentsRequireApproval");
                }

                return Ok(new { });
            }

            return BadRequest(ModelState);
        }
    }
}
