using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Elements.InventoryElements;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    private List<NormalInventoryItem> _visibleSellWindowItems = [];
    private DateTime _nextSellWindowScan = DateTime.MinValue;

    internal List<NormalInventoryItem> GetVisibleSellWindowItems()
    {
        if (!_plugin.GameController.IsUsingController)
        {
            return [];
        }

        if (DateTime.UtcNow < _nextSellWindowScan)
        {
            return _visibleSellWindowItems;
        }

        _nextSellWindowScan = DateTime.UtcNow.AddMilliseconds(1000);

        var ui = _plugin.GameController.IngameState.IngameUi;
        Element sellWindow = null;
        if (ui.SellWindow is { IsValid: true, IsVisible: true })
        {
            sellWindow = ui.SellWindow;
        }

        if (sellWindow == null)
        {
            _visibleSellWindowItems = [];
            return _visibleSellWindowItems;
        }

        _visibleSellWindowItems = FindVisibleInventoryItems(sellWindow, 20)
            .DistinctBy(item => item.Address)
            .ToList();
        return _visibleSellWindowItems;
    }
}
