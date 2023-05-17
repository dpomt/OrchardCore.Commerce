using Microsoft.Extensions.Options;
using OrchardCore.Commerce.Abstractions;
using OrchardCore.Commerce.Models;
using OrchardCore.Commerce.MoneyDataType;
using OrchardCore.Commerce.MoneyDataType.Abstractions;
using OrchardCore.Commerce.Settings;
using OrchardCore.Commerce.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.ContentManagement.Utilities;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Commerce.Drivers;

public class PriceTierPartDisplayDriver : ContentPartDisplayDriver<PriceTierPart>
{
    private readonly IMoneyService _moneyService;
    private readonly IOptions<CurrencySettings> _currencyOptions;

    public PriceTierPartDisplayDriver(
        IMoneyService moneyService,
        IOptions<CurrencySettings> currencyOptions)
    {
        _moneyService = moneyService;
        _currencyOptions = currencyOptions;
    }

    public override IDisplayResult Display(PriceTierPart part, BuildPartDisplayContext context) =>
        Initialize<PriceTierPartViewModel>(GetDisplayShapeType(context), viewModel => BuildViewModel(viewModel, part))
            .Location("Detail", "Content:25")
            .Location("Summary", "Meta:10");

    public override IDisplayResult Edit(PriceTierPart part, BuildPartEditorContext context) =>
        Initialize<PriceTierPartViewModel>(GetEditorShapeType(context), viewModel =>
        {
            BuildViewModel(viewModel, part);
            viewModel.Currencies = _moneyService.Currencies;
        });

    public override async Task<IDisplayResult> UpdateAsync(
        PriceTierPart part,
        IUpdateModel updater,
        UpdatePartEditorContext context)
    {
        var viewModel = new PriceTierPartViewModel();
        if (await updater.TryUpdateModelAsync(
                viewModel,
                Prefix,
                viewModel => viewModel.TierValues,
                viewModel => viewModel.TierAmounts,
                viewModel => viewModel.TierCurrencies
                ))
        {
            // Restoring variants so that only the new values are stored.
            part.Tier.RemoveAll();
            viewModel.Tier.RemoveAll();

            foreach (var x in viewModel.TierValues)
            {
                if (x.Value.HasValue && viewModel.TierAmounts.ContainsKey(x.Key) && viewModel.TierAmounts[x.Key].HasValue)
                {
                    int iAmount = viewModel.TierAmounts[x.Key].Value;
                    part.Tier[iAmount] = _moneyService.Create(viewModel.TierValues[x.Key].Value, "EUR");
                }
            }
        }

        return await EditAsync(part, context);
    }

    private void BuildViewModel(PriceTierPartViewModel model, PriceTierPart part)
    {
        model.ContentItem = part.ContentItem;
        model.PriceTierPart = part;

        

        var values = new Dictionary<string, decimal?>();
        var amounts = new Dictionary<string, int?>();
        var currencies = new Dictionary<string, string>();
        var tier = part.Tier ?? new Dictionary<int, Amount>();
        
        if (tier.Count > 0)
        {
            int s = 0;
            foreach (var t in tier)
            {
                string key = "s" + (++s).ToString();
                values[key] = t.Value.Value;
                amounts[key] = t.Key;
                currencies[key] = t.Value.Currency.CurrencyIsoCode;
            }
        }
        else
        {
            for (int s = 1; s <= 5; ++s)
            {
                string key = "s" + s.ToString();
                values[key] = null;
                amounts[key] = null;
                currencies[key] = _currencyOptions.Value.CurrentDisplayCurrency;
            }
        }


        model.InitializeVariants(tier, values, amounts, currencies);
    }
}
