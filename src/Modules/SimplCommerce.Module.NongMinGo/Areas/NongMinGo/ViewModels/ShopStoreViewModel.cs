using System.Collections.Generic;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;

namespace SimplCommerce.Module.NongMinGo.Areas.NongMinGo.ViewModels
{
    public class ShopStoreViewModel
    {
        public IList<WidgetInstanceViewModel> WidgetInstances { get; set; } = new List<WidgetInstanceViewModel>();

        /// <summary>
        /// 指名這個 ShopStore 的Id，如果為Null 就是不指明
        /// </summary>
        public long? ShopStoreId { get; set; }
    }
}
