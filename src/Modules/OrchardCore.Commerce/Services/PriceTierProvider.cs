using OrchardCore.Commerce.Abstractions;
using OrchardCore.Commerce.Extensions;
using OrchardCore.Commerce.Models;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Commerce.Services;

/// <summary>
/// A simple price provider that obtains a price from a product by looking for a `PriceTiersPart`.
/// </summary>
public class PriceTierProvider : IPriceProvider
{
    private readonly IProductService _productService;

    public int Order => 1;

    public PriceTierProvider(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IList<ShoppingCartItem>> UpdateAsync(IList<ShoppingCartItem> model)
    {
        var skuProducts = await _productService.GetSkuProductsAsync(model);

        return model
            .Select(item =>
            {
                if (skuProducts.TryGetValue(item.ProductSku, out var productPart))
                {
                    var itemWithPrice = AddPriceToShoppingCartItem(item, productPart);

                    if (itemWithPrice != null)
                    {
                        return itemWithPrice;
                    }
                }

                return item;
            })
            .ToList();
    }

    private ShoppingCartItem AddPriceToShoppingCartItem(ShoppingCartItem item, ProductPart productPart)
    {
        var priceTierPart = productPart.ContentItem.As<PriceTierPart>();

        if (priceTierPart is { Tier: { } tier } && tier.Any())
        {
            var amount = tier.OrderBy(p => p.Key).Where(p => p.Key <= item.Quantity).Last().Value;
            return item.WithPrice(new PrioritizedPrice(1, amount));

            
        }

        return null;
    }

    private ShoppingCartItem AddPriceToShoppingCartItemXXX(ShoppingCartItem item, ProductPart productPart)
    {
        var priceTiersPart = productPart.ContentItem.As<PriceTierPart>();

        if (priceTiersPart is { Tier: { } tier} && tier.Any())
        {
            var amount = tier.OrderBy(p => p).First(p => p.Key <= item.Quantity).Value;
            return item.WithPrice(new PrioritizedPrice(1, amount));            
        }

        return null;
    }

    public async Task<bool> IsApplicableAsync(IList<ShoppingCartItem> model)
    {
        var skuProducts = await _productService.GetSkuProductsAsync(model);

        return model.All(item =>
                skuProducts.TryGetValue(item.ProductSku, out var productPart) &&
                productPart.ContentItem.Has<PriceTierPart>());
    }
}
