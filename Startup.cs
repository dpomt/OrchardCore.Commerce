using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Commerce.Abstractions;
using OrchardCore.Commerce.Drivers;
using OrchardCore.Commerce.Fields;
using OrchardCore.Commerce.Handlers;
using OrchardCore.Commerce.Indexes;
using OrchardCore.Commerce.Migrations;
using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.Money;
using OrchardCore.Commerce.Services;
using OrchardCore.Commerce.Settings;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.Data.Migration;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;
using YesSql.Indexes;

namespace OrchardCore.Commerce
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            // Product
            services.AddSingleton<IIndexProvider, ProductPartIndexProvider>();
            services.AddScoped<IDataMigration, ProductMigrations>();
            services.AddScoped<IContentAliasProvider, ProductPartContentAliasProvider>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IContentPartDisplayDriver, ProductPartDisplayDriver>();
            services.AddSingleton<ContentPart, ProductPart>();
            // Attributes
            services.AddSingleton<ContentField, BooleanProductAttributeField>();
            services.AddScoped<IContentFieldDisplayDriver, BooleanProductAttributeFieldDriver>();
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, BooleanProductAttributeFieldSettingsDriver>();
            services.AddSingleton<ContentField, NumericProductAttributeField>();
            services.AddScoped<IContentFieldDisplayDriver, NumericProductAttributeFieldDriver>();
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, NumericProductAttributeFieldSettingsDriver>();
            services.AddSingleton<ContentField, TextProductAttributeField>();
            services.AddScoped<IContentFieldDisplayDriver, TextProductAttributeFieldDriver>();
            services.AddScoped<IContentPartFieldDefinitionDisplayDriver, TextProductAttributeFieldSettingsDriver>();
            services.AddScoped<IProductAttributeParseService, ProductAttributeParseService>();
            // Price
            services.AddScoped<IDataMigration, PriceMigrations>();
            services.AddScoped<IContentPartHandler, PricePartHandler>();
            services.AddScoped<IContentPartDisplayDriver, PricePartDisplayDriver>();
            services.AddSingleton<ContentPart, PricePart>();
            services.AddSingleton<IPriceProvider, PriceProvider>();
            services.AddSingleton<IPriceService, PriceService>();
            // Currency
            services.AddSingleton<ICurrencyProvider, CurrencyProvider>();
            services.AddSingleton<IMoneyService, MoneyService>();
            // Shopping cart
            services.AddSingleton<IShoppingCartHelpers, ShoppingCartHelpers>();
            // Settings
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IDisplayDriver<ISite>, CommerceSettingsDisplayDriver>();
            services.AddScoped<INavigationProvider, AdminMenu>();
        }
    }

    [RequireFeatures(CommerceConstants.Features.SessionCartStorage)]
    public class SessionCartStorageStartup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSession(options => { });
            // Shopping Cart
            services.AddSingleton<IShoppingCartPersistence, SessionShoppingCartPersistence>();
        }

        public override void Configure(IApplicationBuilder app, IRouteBuilder routes, IServiceProvider serviceProvider)
        {
            app.UseSession();
            routes.MapAreaRoute(
                name: "ShoppingCart",
                areaName: "OrchardCore.Commerce",
                template: "shoppingcart/{action}",
                defaults: new { controller = "ShoppingCart", action = "Index" }
            );
        }
    }
}
