using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimplCommerce.Module.Catalog.Areas.Catalog.ViewModels
{
    public class ProductWidgetSetting
    {
        public int NumberOfProducts { get; set; }

        public long? CategoryId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProductWidgetOrderBy OrderBy { get; set; }

        public bool FeaturedOnly { get; set; }

        /// <summary>
        /// 指名這個 ShopStore 的Id，如果為Null 就是不指明
        /// </summary>
        public long? ShopStoreId { get; set; }
    }
}
