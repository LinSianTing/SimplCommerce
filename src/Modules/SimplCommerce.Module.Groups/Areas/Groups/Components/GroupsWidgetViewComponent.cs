﻿using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Groups.Areas.Groups.ViewModels;
//using SimplCommerce.Module.Groups.Models;
//using SimplCommerce.Module.Groups.Services;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;
using SimplCommerce.Module.Core.Services;
using System.Collections.Generic;

namespace SimplCommerce.Module.Groups.Areas.Groups.Components
{
    public class GroupsWidgetViewComponent : ViewComponent
    {
        public GroupsWidgetViewComponent()
        {

        }

        public IViewComponentResult Invoke(WidgetInstanceViewModel widgetInstance)
        {
            var model = new GroupsWidgetComponentVm
            {
                Id = widgetInstance.Id,
                WidgetName = widgetInstance.Name
                //Setting = JsonConvert.DeserializeObject<ProductWidgetSetting>(widgetInstance.Data)
            };

            List<string> listGroups = new List<string>() { "深溝社群", "慢島生活" , "主婦聯盟"};
            model.Groups = listGroups;

            return View(this.GetViewPath(), model);
        }

        //private readonly IRepository<Product> _productRepository;
        //private readonly IMediaService _mediaService;
        //private readonly IProductPricingService _productPricingService;
        //private readonly IContentLocalizationService _contentLocalizationService;

        /*
    public GroupsWidgetViewComponent(IRepository<Product> productRepository,
        IMediaService mediaService,
        IProductPricingService productPricingService,
        IContentLocalizationService contentLocalizationService)
    {
        _productRepository = productRepository;
        _mediaService = mediaService;
        _productPricingService = productPricingService;
        _contentLocalizationService = contentLocalizationService;
    }

    public IViewComponentResult Invoke(WidgetInstanceViewModel widgetInstance)
    {
        var model = new ProductWidgetComponentVm
        {
            Id = widgetInstance.Id,
            WidgetName = widgetInstance.Name,
            Setting = JsonConvert.DeserializeObject<ProductWidgetSetting>(widgetInstance.Data)
        };

        var query = _productRepository.Query()
          .Where(x => x.IsPublished && x.IsVisibleIndividually);

        if (model.Setting.CategoryId.HasValue && model.Setting.CategoryId.Value > 0)
        {
            query = query.Where(x => x.Categories.Any(c => c.CategoryId == model.Setting.CategoryId.Value));
        }

        if (model.Setting.FeaturedOnly)
        {
            query = query.Where(x => x.IsFeatured);
        }

        model.Products = query
          .Include(x => x.ThumbnailImage)
          .OrderByDescending(x => x.CreatedOn)
          .Take(model.Setting.NumberOfProducts)
          .Select(x => ProductThumbnail.FromProduct(x)).ToList();

        foreach (var product in model.Products)
        {
            product.Name = _contentLocalizationService.GetLocalizedProperty(nameof(Product), product.Id, nameof(product.Name), product.Name);
            product.ThumbnailUrl = _mediaService.GetThumbnailUrl(product.ThumbnailImage);
            product.CalculatedProductPrice = _productPricingService.CalculateProductPrice(product);
        }

        return View(this.GetViewPath(), model);
    }
    */
    }
}
