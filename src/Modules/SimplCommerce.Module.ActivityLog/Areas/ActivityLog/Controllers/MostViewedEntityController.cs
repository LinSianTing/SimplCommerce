using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.ActivityLog.Data;
using SimplCommerce.Module.ActivityLog.Models;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Extensions;

namespace SimplCommerce.Module.ActivityLog.Areas.ActivityLog.Controllers
{
    [Area("ActivityLog")]
    [Authorize(Roles = "admin,vendor, farmer")]
    [Route("api/activitylog")]
    public class MostViewedEntityController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IActivityTypeRepository _activityTypeRepository;
        private readonly IWorkContext _workContext;

        public MostViewedEntityController(IRepository<Product> productRepository, IActivityTypeRepository activityTypeRepository,IWorkContext workContext)
        {
            _productRepository = productRepository;
            _activityTypeRepository = activityTypeRepository;
            _workContext = workContext;
        }

        [HttpGet("most-viewed-entities/{entityTypeId}")]
        public async Task<IList<MostViewEntityDto>> GetMostViewedEntities(string entityTypeId)
        {
            if (User.IsInRole("admin"))
            {
                return await _activityTypeRepository.List().Where(x => x.EntityTypeId == entityTypeId).Take(10).ToListAsync();
            }
            else
            {
                var currentUser = await _workContext.GetCurrentUser();

                var query = _productRepository.Query().Where(x => x.BrandId == currentUser.Id);

                var thisFarmerProductList = query.Select(a => a.Id);

                return await _activityTypeRepository.List().Where(x => x.EntityTypeId == entityTypeId && thisFarmerProductList.Contains(x.EntityId)).Take(10).ToListAsync();
            }
        }
    }
}
