using OrchardCore.Commerce.MoneyDataType;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;

namespace OrchardCore.Commerce.Models;

/// <summary>
/// A product variants prices based on predefined attributes.
/// </summary>
public class PriceTierPart : ContentPart
{
    public IDictionary<int, Amount> Tier { get; } = new Dictionary<int, Amount>();
}
