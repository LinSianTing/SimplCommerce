using System.Collections.Generic;

namespace SimplCommerce.Module.Groups.Areas.Groups.ViewModels
{
    public class GraphWidgetComponentVm
    {
        public long Id { get; set; }

        public string WidgetName { get; set; }

        //public ProductWidgetSetting Setting { get; set; }

        public List<string> Groups { get; set; }
    }
}
