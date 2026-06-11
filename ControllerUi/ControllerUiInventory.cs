using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Elements.InventoryElements;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    private List<NormalInventoryItem> _visibleInventoryItems = [];
    private DateTime _nextInventoryScan = DateTime.MinValue;

    internal List<NormalInventoryItem> GetVisibleInventoryItems()
    {
        if (!_plugin.GameController.IsUsingController)
        {
            return [];
        }

        var inventory = _plugin.GameController.IngameState.IngameUi.InventoryPanel;
        if (inventory?.IsValid != true || !inventory.IsVisible)
        {
            _visibleInventoryItems = [];
            return _visibleInventoryItems;
        }

        if (DateTime.UtcNow < _nextInventoryScan)
        {
            return _visibleInventoryItems;
        }

        _nextInventoryScan = DateTime.UtcNow.AddMilliseconds(250);
        _visibleInventoryItems = FindInventoryGrid(inventory, 8)?
            .Children
            .Select(AsInventoryItem)
            .Where(item => item?.Item?.IsValid == true && !string.IsNullOrWhiteSpace(item.Item.Path))
            .DistinctBy(item => item.Address)
            .ToList() ?? [];
        return _visibleInventoryItems;
    }

    private static Element FindInventoryGrid(Element element, int depth)
    {
        if (element?.IsValid != true || !element.IsVisible || depth < 0)
        {
            return null;
        }

        var candidates = new List<Element>();
        if (element.Children.Any(child => AsInventoryItem(child)?.Item?.IsValid == true))
        {
            candidates.Add(element);
        }

        foreach (var child in element.Children)
        {
            var candidate = FindInventoryGrid(child, depth - 1);
            if (candidate != null)
            {
                candidates.Add(candidate);
            }
        }

        return candidates
            .OrderByDescending(candidate => candidate.Width * candidate.Height)
            .FirstOrDefault();
    }
}
