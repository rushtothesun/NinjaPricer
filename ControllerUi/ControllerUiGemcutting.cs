using System;
using System.Linq;
using ExileCore2.PoEMemory;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    private Element _gemcuttingWindow;
    private DateTime _nextGemcuttingWindowScan = DateTime.MinValue;

    internal bool IsGemcuttingWindowVisible()
    {
        if (!_plugin.GameController.IsUsingController)
        {
            return false;
        }

        if (_gemcuttingWindow?.IsValid == true)
        {
            return _gemcuttingWindow.IsVisible;
        }

        if (DateTime.UtcNow < _nextGemcuttingWindowScan)
        {
            return false;
        }

        _nextGemcuttingWindowScan = DateTime.UtcNow.AddMilliseconds(1000);
        var ui = _plugin.GameController.IngameState.IngameUi;
        _gemcuttingWindow = ui?
            .Children
            .FirstOrDefault(IsGemcuttingWindow);
        return _gemcuttingWindow?.IsVisible == true;
    }

    private static bool IsGemcuttingWindow(Element element)
    {
        return element?.IsValid == true &&
               element.ChildCount == 3 &&
               element.GetChildAtIndex(0) is { IsValid: true, ChildCount: 11 };
    }
}
