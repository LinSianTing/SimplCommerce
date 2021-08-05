using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

namespace SimplCommerce.WebHost.Migrations
{
    public partial class AddedGeneralSystemIdColumn : Migration
    {
        private List<string> reviseTables = new List<string>() {
            //SimplCommerce.Infrastructure
            "Localization_LocalizedContentProperty",
            "Localization_Resource",
            //SimplCommerce.Module.ActivityLog
            "ActivityLog_Activity",
            "ActivityLog_ActivityType",
            //SimplCommerce.Module.Catalog
            "Catalog_Brand",
            "Catalog_Category",
            "Catalog_Product",
            "Catalog_ProductAttribute",
            "Catalog_ProductAttributeGroup",
            "Catalog_ProductAttributeValue",
            "Catalog_ProductCategory",
            "Catalog_ProductLink",
            "Catalog_ProductMedia",
            "Catalog_ProductOption",
            "Catalog_ProductOptionCombination",
            "Catalog_ProductOptionValue",
            "Catalog_ProductPriceHistory",
            "Catalog_ProductTemplate",
            //SimplCommerce.Module.Cms
            "Cms_Menu",
            "Cms_MenuItem",
            "Cms_Page",
            //SimplCommerce.Module.Comments
            "Comments_Comment",
            //SimplCommerce.Module.Contacts
            "Contacts_Contact",
            "Contacts_ContactArea",
            //SimplCommerce.Module.Core
            "Core_Address",
            "Core_CustomerGroup",
            "Core_District",
            "Core_Entity",
            "Core_Media",
            "Core_StateOrProvince",
            "Core_UserAddress",
            "Core_Vendor",
            "Core_WidgetInstance",
            "Core_WidgetZone",
            //SimplCommerce.Module.Inventory
            "Inventory_Stock",
            "Inventory_StockHistory",
            "Inventory_Warehouse",
            //SimplCommerce.Module.News
            "News_NewsCategory",
            "News_NewsItem",
            //SimplCommerce.Module.Orders
            "Orders_Order",
            "Orders_OrderAddress",
            "Orders_OrderHistory",
            "Orders_OrderItem",
            //SimplCommerce.Module.Payments
            "Payments_Payment",
            //SimplCommerce.Module.Pricing
            "Pricing_CartRule",
            "Pricing_CartRuleUsage",
            "Pricing_CatalogRule",
            "Pricing_Coupon",
            //SimplCommerce.Module.ProductComparison
            "ProductComparison_ComparingProduct",
            //SimplCommerce.Module.ProductRecentlyViewed
            "ProductRecentlyViewed_RecentlyViewedProduct",
            //SimplCommerce.Module.Reviews
            "Reviews_Reply",
            "Reviews_Review",
            //SimplCommerce.Module.Search
            "Search_Query",
            //SimplCommerce.Module.Shipments
            "Shipments_Shipment",
            "Shipments_ShipmentItem",
            //SimplCommerce.Module.ShippingTableRate
            "ShippingTableRate_PriceAndDestination",
            //SimplCommerce.Module.ShoppingCart
            "ShoppingCart_Cart",
            "ShoppingCart_CartItem",
            //SimplCommerce.Module.Tax
            "Tax_TaxClass",
            "Tax_TaxRate",
            //SimplCommerce.Module.WishList
            "WishList_WishList",
            "WishList_WishListItem"
            };

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (string tablename in reviseTables)
            {
                migrationBuilder.AddColumn<int>(
                        name: "SystemId",
                        table: tablename,
                        nullable: true,
                        defaultValue: 0);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (string tablename in reviseTables)
            {
                migrationBuilder.DropColumn(
                    name: "SystemId",
                    table: tablename);
            }
        }
    }
}
