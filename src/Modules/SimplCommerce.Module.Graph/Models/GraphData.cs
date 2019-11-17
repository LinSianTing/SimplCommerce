using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.Graph.Models
{
    public class GraphData
    {
        public List<NodeData> nodes { get; set; }
        public List<LinkData> links { get; set; }
    }
}
