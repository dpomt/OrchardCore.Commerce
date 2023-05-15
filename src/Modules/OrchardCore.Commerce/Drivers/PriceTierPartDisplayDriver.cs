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
                viewModel => viewModel.TierCurrency))
        {
            // Restoring variants so that only the new values are stored.
            part.Tier.RemoveAll();
            viewModel.Tier.RemoveAll();

            foreach (var x in viewModel.TierValues)
            {
                if (x.Value.HasValue && viewModel.TierCurrency != Currency.UnspecifiedCurrency.CurrencyIsoCode)
                {
                    part.Tier[x.Key] = _moneyService.Create(x.Value.Value, viewModel.TierCurrency);
                }
            }
        }

        return await EditAsync(part, context);
    }

    private void BuildViewModel(PriceTierPartViewModel model, PriceTierPart part)
    {
        model.ContentItem = part.ContentItem;
        model.PriceTierPart = part;

        var tier = part.Tier ?? new Dictionary<int, Amount>();
        var values = new Dictionary<int, decimal?>();
        for (int i = 0; i < 5; ++i)
        {
            values[i] = null;
        }
        //tier.ForEach(p => values[p.Key] = p.Value.Value);
        model.InitializeVariants(tier, values, model.TierCurrency);
    }
}
