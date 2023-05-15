using Microsoft.AspNetCore.Mvc.ModelBinding;
using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.MoneyDataType;
using OrchardCore.Commerce.MoneyDataType.Abstractions;
using OrchardCore.ContentManagement;
using System.Collections.Generic;

namespace OrchardCore.Commerce.ViewModels;

public class PriceTierPartViewModel
{
    public IDictionary<int, decimal?> TierValues { get; private set; } = new Dictionary<int, decimal?>();
    public string TierCurrency { get; private set; }
    
    public IEnumerable<ICurrency> Currencies { get; set; }

    [BindNever]
    public ContentItem ContentItem { get; set; }

    [BindNever]
    public PriceTierPart PriceTierPart { get; set; }

    [BindNever]
    public IDictionary<int, Amount> Tier { get; private set; } = new Dictionary<int, Amount>();

    public void InitializeVariants(
        IDictionary<int, Amount> tier,
        IDictionary<int, decimal?> values,
        string currency)
    {
        Tier = tier;
        TierValues = values;
        TierCurrency = currency;
    }
}
