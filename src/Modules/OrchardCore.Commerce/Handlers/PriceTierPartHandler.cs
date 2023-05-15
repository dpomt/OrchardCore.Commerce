using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.MoneyDataType.Abstractions;
using OrchardCore.ContentManagement.Handlers;
using System.Threading.Tasks;

namespace OrchardCore.Commerce.Handlers;

public class PriceTierPartHandler : ContentPartHandler<PriceTierPart>
{
    private readonly IMoneyService _moneyService;

    public PriceTierPartHandler(IMoneyService moneyService) => _moneyService = moneyService;

    public override Task LoadingAsync(LoadContentContext context, PriceTierPart instance)
    {
        if (instance.Tier != null)
        {
            foreach (var variantKey in instance.Tier.Keys)
            {
                instance.Tier[variantKey] = _moneyService.EnsureCurrency(instance.Tier[variantKey]);
            }
        }

        return base.LoadingAsync(context, instance);
    }
}
