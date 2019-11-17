using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Module.Graph.Models
{
    public class NodeData
    {
        public long id { get; set; }
        public string name { get; set; }
        public int point { get; set; }
        public int buy { get; set; }
        public int sell { get; set; }
        public int newproducts { get; set; }
        public string group { get; set; }
        public int color { get; set; }
    }
}
