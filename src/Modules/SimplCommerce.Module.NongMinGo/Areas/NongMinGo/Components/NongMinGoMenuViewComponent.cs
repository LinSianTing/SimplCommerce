using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.NongMinGo.Areas.NongMinGo.ViewModels;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.NongMinGo.Areas.NongMinGo.Components
{
    public class NongMinGoMenuViewComponent : ViewComponent
    {
        //private readonly IRepository<NongMinGo> _categoryRepository;
        //private readonly IContentLocalizationService _contentLocalizationService;

        public NongMinGoMenuViewComponent()
        {
            //_categoryRepository = categoryRepository;
            //_contentLocalizationService = contentLocalizationService;
        }

        public IViewComponentResult Invoke()
        {
            //var categories = _categoryRepository.Query().Where(x => !x.IsDeleted && x.IncludeInMenu).ToList();

            //var categoryMenuItems = new List<CategoryMenuItem>();
            //var topCategories = categories.Where(x => !x.ParentId.HasValue).OrderByDescending(x => x.DisplayOrder);
            //foreach (var category in topCategories)
            //{
            //    var categoryMenuItem = Map(category);
            //    categoryMenuItems.Add(categoryMenuItem);
            //}

            return View(this.GetViewPath());
        }

        //private NongMinGoMenuItem Map(Category category)
        //{ 
        //    var categoryMenuItem = new CategoryMenuItem
        //    //var categoryMenuItem = new CategoryMenuItem
        //    //{
        //    //    Id = category.Id,
        //    //    Name = _contentLocalizationService.GetLocalizedProperty(category, nameof(category.Name), category.Name),
        //    //    Slug = category.Slug
        //    //};

        //    //var childCategories = category.Children;
        //    //foreach (var childCategory in childCategories.OrderByDescending(x => x.DisplayOrder))
        //    //{
        //    //    var childCategoryMenuItem = Map(childCategory);
        //    //    categoryMenuItem.AddChildItem(childCategoryMenuItem);
        //    //}

        //    return categoryMenuItem;
        //}
    }
}
