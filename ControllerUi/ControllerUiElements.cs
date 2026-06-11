using System;
using System.Collections.Generic;
using ExileCore2.PoEMemory;
using ExileCore2.PoEMemory.Elements.InventoryElements;

namespace NinjaPricer;

internal sealed partial class ControllerUi
{
    private static IEnumerable<Element> FindVisibleTextElements(Element element, int depth)
    {
        if (element?.IsValid != true || !element.IsVisible || depth < 0)
        {
            yield break;
        }

        if (!string.IsNullOrWhiteSpace(element.TextNoTags ?? element.Text))
        {
            yield return element;
        }

        foreach (var child in element.Children)
        {
            foreach (var found in FindVisibleTextElements(child, depth - 1))
            {
                yield return found;
            }
        }
    }

    private static bool ContainsVisibleText(Element element, int depth, string text)
    {
        if (element?.IsValid != true || !element.IsVisible || depth < 0)
        {
            return false;
        }

        if ((element.TextNoTags ?? element.Text)?.Contains(text, StringComparison.Ordinal) == true)
        {
            return true;
        }

        foreach (var child in element.Children)
        {
            if (ContainsVisibleText(child, depth - 1, text))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<NormalInventoryItem> FindVisibleInventoryItems(Element element, int depth)
    {
        if (element?.IsValid != true || !element.IsVisible || depth < 0)
        {
            yield break;
        }

        NormalInventoryItem item = null;
        if (IsInventoryItemSizeCandidate(element))
        {
            try
            {
                item = element.AsObject<NormalInventoryItem>();
            }
            catch
            {
            }
        }

        if (item?.Item?.IsValid == true && !string.IsNullOrWhiteSpace(item.Item.Path))
        {
            yield return item;
        }

        foreach (var child in element.Children)
        {
            foreach (var found in FindVisibleInventoryItems(child, depth - 1))
            {
                yield return found;
            }
        }
    }

    private static NormalInventoryItem AsInventoryItem(Element element)
    {
        if (!IsInventoryItemSizeCandidate(element))
        {
            return null;
        }

        try
        {
            return element?.AsObject<NormalInventoryItem>();
        }
        catch
        {
            return null;
        }
    }

    private static bool IsInventoryItemSizeCandidate(Element element)
    {
        return element?.Width >= 50 && element.Height >= 50;
    }
}
