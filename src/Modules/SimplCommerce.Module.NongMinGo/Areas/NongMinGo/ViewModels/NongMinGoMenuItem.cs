using System.Collections.Generic;

namespace SimplCommerce.Module.NongMinGo.Areas.NongMinGo.ViewModels
{
    public class NongMinGoMenuItem
    {
        public NongMinGoMenuItem()
        {
            ChildItems = new List<NongMinGoMenuItem>();
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public NongMinGoMenuItem Parent { get; set; }

        public IList<NongMinGoMenuItem> ChildItems { get; set; }

        public void AddChildItem(NongMinGoMenuItem childItem)
        {
            childItem.Parent = this;
            ChildItems.Add(childItem);
        }
    }
}
