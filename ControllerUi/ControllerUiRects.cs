using System.Linq;
using System.Numerics;
using ExileCore2.PoEMemory;
using ExileCore2.Shared;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    internal RectangleF? GetRect(Element element)
    {
        if (!_plugin.GameController.IsUsingController || element?.IsValid != true)
        {
            return null;
        }

        var rect = element.GetClientRectCache;
        if (IsUsableRect(rect) && IsFiniteRect(rect))
        {
            return rect;
        }

        return TryGetControllerRect(element, GetControllerUiRoot(), out rect)
            ? rect
            : null;
    }

    internal RectangleF? GetItemTooltipHeaderTextRect(Element tooltip)
    {
        if (!_plugin.GameController.IsUsingController || tooltip?.IsValid != true || !tooltip.IsVisible)
        {
            return null;
        }

        return FindVisibleTextElements(tooltip, 4)
            .Select(GetRect)
            .Where(rect => rect != null)
            .OrderBy(rect => rect.Value.Top)
            .FirstOrDefault();
    }

    private Element GetControllerUiRoot()
    {
        var menu = _plugin.GameController.IngameState.IngameUi.ControllerGeneralMenu;
        return menu is { IsValid: true, IsVisible: true, ChildCount: > 0 }
            ? menu
            : null;
    }

    private bool TryGetControllerRect(Element element, Element controllerRoot, out RectangleF rect)
    {
        rect = default;
        if (controllerRoot?.IsValid != true || controllerRoot.Height <= 0)
        {
            return false;
        }

        var position = Vector2.Zero;
        var current = element;
        for (var depth = 0; current?.IsValid == true && depth < 32; depth++)
        {
            position += current.Position;
            if (current.Address == controllerRoot.Address)
            {
                break;
            }

            current = current.Parent;
        }

        if (current?.Address != controllerRoot.Address)
        {
            return false;
        }

        var window = _plugin.GameController.Window.GetWindowRectangle() with { Location = Vector2.Zero };
        var uiScale = window.Height / controllerRoot.Height;
        var scaledRootSize = new Vector2(controllerRoot.Width, controllerRoot.Height) * uiScale;
        var uiOffset = (window.Size - scaledRootSize) / 2;
        var topLeft = position * uiScale + uiOffset;

        rect = new RectangleF(
            topLeft.X,
            topLeft.Y,
            element.Width * uiScale,
            element.Height * uiScale);

        return IsUsableRect(rect) && IsFiniteRect(rect);
    }

    private static bool IsUsableRect(RectangleF rect)
    {
        return rect.Width > 0 && rect.Height > 0;
    }

    private static bool IsFiniteRect(RectangleF rect)
    {
        return float.IsFinite(rect.X) &&
               float.IsFinite(rect.Y) &&
               float.IsFinite(rect.Width) &&
               float.IsFinite(rect.Height);
    }
}
