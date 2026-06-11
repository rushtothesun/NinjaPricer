using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore2.PoEMemory.Elements.InventoryElements;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    private List<NormalInventoryItem> _visibleStashItems = [];
    private DateTime _nextStashScan = DateTime.MinValue;

    internal List<NormalInventoryItem> GetVisibleStashItems()
    {
        if (!_plugin.GameController.IsUsingController)
        {
            return [];
        }

        var stash = _plugin.GameController.IngameState.IngameUi.StashElement;
        if (stash?.IsValid != true || !stash.IsVisible)
        {
            _visibleStashItems = [];
            return _visibleStashItems;
        }

        if (DateTime.UtcNow < _nextStashScan)
        {
            return _visibleStashItems;
        }

        _nextStashScan = DateTime.UtcNow.AddMilliseconds(250);
        _visibleStashItems = FindVisibleInventoryItems(stash, 20)
            .DistinctBy(item => item.Address)
            .ToList();
        return _visibleStashItems;
    }
}
