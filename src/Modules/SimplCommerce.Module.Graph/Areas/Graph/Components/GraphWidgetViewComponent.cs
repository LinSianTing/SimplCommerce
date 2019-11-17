using System.Linq;
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
using SimplCommerce.Module.Graph.Models;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Extensions;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Catalog.Models;
using System;

namespace SimplCommerce.Module.Graph.Areas.Graph.Components
{
    public class GraphWidgetViewComponent : ViewComponent
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Connection> _connectionRepository;
        private readonly IConnectionService _connectionService;
        private readonly IWorkContext _workContext;

        public GraphWidgetViewComponent(
            IRepository<Order> orderRepository, 
            IRepository<User> userRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
        UserManager<User> userManager, 
            IWorkContext workContext, IRepository<Connection> connectionRepository,
            IConnectionService connectionService)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _workContext = workContext;
            _connectionRepository = connectionRepository;
            _connectionService = connectionService;
        }

        public async Task<IViewComponentResult> InvokeAsync(WidgetInstanceViewModel widgetInstance)
        {
            var model = new GraphWidgetComponentVm
            {
                Id = widgetInstance.Id,
                WidgetName = widgetInstance.Name
                //Setting = JsonConvert.DeserializeObject<ProductWidgetSetting>(widgetInstance.Data)
            };

            List<string> listGroups = new List<string>() { "深溝社群", "員山農會", "新青農陣線" };
            model.Groups = listGroups;

            //IQueryable<User> users =  _userManager.Users;
            List<User> users = null;

            using (var transaction = _userRepository.BeginTransaction())
            {
                users = _userRepository.Query().Where(a => a.VendorId != null).ToList();
            }

            GraphData graphData = new GraphData();

            var nodeList = await GetNodeDatas(users);
            graphData.nodes = nodeList;

            if(!string.IsNullOrEmpty(widgetInstance.UrlParameter))
            {
                graphData.nodes = graphData.nodes.Where(a => a.group.Contains(widgetInstance.UrlParameter.Substring(0, 1))).ToList();
            }

            graphData.links = await GetLinkDatas(users, graphData.nodes);

            model.GraphData = graphData;
            model.GraphDataStr = JsonConvert.SerializeObject(graphData);
            //var encodedOutput = HtmlEncoder.Default.Encode(model.GraphDataStr);
            //model.GraphDataStr = encodedOutput;
            return View(this.GetViewPath(), model);
        }

        private async Task<List<NodeData>> GetNodeDatas(List<User> users)
        {
            User user =  await _workContext.GetCurrentUser();

            var query = _connectionRepository.Query();

            query = query.Where(a => a.Target == user.Id || a.Source == user.Id);

            List<NodeData> result = new List<NodeData>();

            result.Add(new NodeData()
            {
                id = user.Id,
                name = user.FullName =="Guest"?"農民GO用戶": user.FullName,
                point = 10,
                buy = 0,
                sell = 0,
                group = string.Empty,
                color = 7
            }); ;

            List<Product> products = _productRepository.Query().Where(a=>!a.IsDeleted && a.IsPublished).ToList();

            int indexoFStr = 0;
            var stringGroupArray = new string[5] { "", "深溝", "", "員山" , "新青" };
            var intColorArray = new int[5] {3,6,9,12,15 };
            Random random = new Random();
             foreach (Connection connection in query)
            {
                if ((user.Id == connection.Target) || (user.Id == connection.Source))
                {
                    long thisUserId = 0;
                    if (user.Id == connection.Target)
                    {
                        thisUserId = connection.Source;
                    }
                    else
                    {
                        thisUserId = connection.Target;
                    }
                    string thisUserName = users.Where(a => a.Id == thisUserId).Select(a => a.FullName).FirstOrDefault();

                    if (result.Count(a => a.id == thisUserId) == 0)
                    {
                        int randomIndex = random.Next(0, 4);
                        result.Add(new NodeData()
                        {
                            id = thisUserId,
                            name = thisUserName, //users.Where(a => a.Id == thisUserId).Select(a => a.FullName).FirstOrDefault(),
                            buy = 0,
                            sell = 0,
                            newproducts = products.Count(a=>a.BrandId == thisUserId),
                            group = stringGroupArray[indexoFStr],
                            color = intColorArray[indexoFStr]
                        }); ; ;

                        indexoFStr++;
                        if(indexoFStr>=stringGroupArray.Length)
                        {
                            indexoFStr = 0;
                        }
                    }
                }
            }

            return result;
        }

        private async Task<List<LinkData>> GetLinkDatas(List<User> users, List<NodeData> nodes)
        {
            User user = user = await _workContext.GetCurrentUser();


            List<LinkData> result = new List<LinkData>();

            List<long> yourNodesIds = nodes.Select(a=>a.id).ToList();
            List<Order> orders = _orderRepository.Query().Where(a=>a.IsMasterOrder).ToList();

            List<Order> customerOrders = orders.Where(a => yourNodesIds.Contains(a.CustomerId)).ToList();
            List<Order> youOrders = orders.Where(a =>a.CustomerId == user.Id).ToList();


            List<Product> yourProducts = _productRepository.Query().Where(a => a.BrandId == user.Id).ToList();
            List<long> yourProductIds = yourProducts.Select(a => a.Id).ToList();
            List<OrderItem> orderItems = _orderItemRepository.Query().Where(a => yourProductIds.Contains(a.ProductId) && customerOrders.Select(b=>b.Id).Contains(a.Order.Id)).ToList();

            // order to you
            foreach (Order customerOrder in customerOrders)
            {
                var thisOrderItems = orderItems.Where( b => b.Order.Id == customerOrder.Id);
                foreach (OrderItem customerOrderItem in thisOrderItems)
                {
                    if (yourProductIds.Contains(customerOrderItem.ProductId))
                    {
                        if(result.Count(a=>a.source == customerOrder.CustomerId)==0)
                        {
                            // your customer
                            LinkData linkData = new LinkData()
                            {
                                source = customerOrder.CustomerId,
                                target = user.Id,
                                value = 3
                            };
                            result.Add(linkData);

                            NodeData nodeData = nodes.Where(a => a.id == customerOrder.CustomerId).FirstOrDefault();
                            nodeData.buy = nodeData.buy + 1;
                        }
                        else
                        {
                            LinkData linkData = result.Where(a => a.source == customerOrder.CustomerId).FirstOrDefault();
                            linkData.value = linkData.value + 1;

                            NodeData nodeData = nodes.Where(a => a.id == customerOrder.CustomerId).FirstOrDefault();
                            nodeData.buy = nodeData.buy + 1;
                        }
                    }
                }
            }

            return result;
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
