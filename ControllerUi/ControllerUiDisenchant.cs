using System;
using ExileCore2.PoEMemory;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    private Element _disenchantWindow;
    private DateTime _nextDisenchantWindowScan = DateTime.MinValue;

    internal bool IsDisenchantWindowVisible()
    {
        if (!_plugin.GameController.IsUsingController)
        {
            return false;
        }

        if (_disenchantWindow?.IsValid == true)
        {
            return _disenchantWindow.IsVisible;
        }

        if (DateTime.UtcNow < _nextDisenchantWindowScan)
        {
            return false;
        }

        _nextDisenchantWindowScan = DateTime.UtcNow.AddMilliseconds(1000);
        var ui = _plugin.GameController.IngameState.IngameUi;
        _disenchantWindow = FindVisibleElement(ui, 6, IsDisenchantWindow);
        return _disenchantWindow?.IsVisible == true;
    }

    private static bool IsDisenchantWindow(Element element)
    {
        if (element?.IsValid != true ||
            !element.IsVisible ||
            element.ChildCount != 1 ||
            element.GetChildAtIndex(0) is not { IsValid: true, ChildCount: 5 })
        {
            return false;
        }

        return ContainsVisibleText(element, 5, "Place your items here to disenchant them");
    }

    private static Element FindVisibleElement(Element element, int depth, Func<Element, bool> predicate)
    {
        if (element?.IsValid != true || !element.IsVisible || depth < 0)
        {
            return null;
        }

        if (predicate(element))
        {
            return element;
        }

        foreach (var child in element.Children)
        {
            var found = FindVisibleElement(child, depth - 1, predicate);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
