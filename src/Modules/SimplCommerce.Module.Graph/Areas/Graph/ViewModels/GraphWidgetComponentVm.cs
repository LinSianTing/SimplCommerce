using System.Collections.Generic;
using SimplCommerce.Module.Graph.Models;

namespace SimplCommerce.Module.Groups.Areas.Groups.ViewModels
{
    public class GraphWidgetComponentVm
    {
        public long Id { get; set; }

        public string WidgetName { get; set; }

        //public ProductWidgetSetting Setting { get; set; }

        public List<string> Groups { get; set; }

        public GraphData GraphData { get; set; }
        public string GraphDataStr { get; set; }
    }
}
