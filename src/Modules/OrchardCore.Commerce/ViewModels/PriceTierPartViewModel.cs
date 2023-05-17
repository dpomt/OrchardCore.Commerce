using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.MoneyDataType;
using OrchardCore.Commerce.MoneyDataType.Abstractions;
using OrchardCore.ContentManagement;
using System.Collections.Generic;

namespace OrchardCore.Commerce.ViewModels;

public class PriceTierPartViewModel
{
    public IDictionary<string, decimal?> TierValues { get; private set; } = new Dictionary<string, decimal?>();
    public IDictionary<string, int?> TierAmounts{ get; private set; } = new Dictionary<string, int?>();
    public IDictionary<string, string> TierCurrencies { get; private set; } = new Dictionary<string, string>();

    public IEnumerable<ICurrency> Currencies { get; set; }

    [BindNever]
    public ContentItem ContentItem { get; set; }

    [BindNever]
    public PriceTierPart PriceTierPart { get; set; }

    [BindNever]
    public IDictionary<int, Amount> Tier { get; private set; } = new Dictionary<int, Amount>();

    public void InitializeVariants(
        IDictionary<int, Amount> tier,
        IDictionary<string, decimal?> values,
        IDictionary<string, int?> amounts,
        IDictionary<string, string> currencies)
    {
        Tier = tier;
        TierValues = values;
        TierAmounts = amounts;
        TierCurrencies = currencies;
    }
}
