using System;

namespace NinjaPricer;

public static class Extensions
{
    public static string FormatNumber(this double number, int significantDigits, double maxInvertValue = 0, bool forceDecimals = false)
    {
        if (TryFormatEdgeCase(number, out var formatNumber)) return formatNumber;

        if (number == 0)
        {
            return "0";
        }

        if (Math.Abs(number) <= 1e-10)
        {
            return "~0";
        }

        if (Math.Abs(number) < maxInvertValue)
        {
            var inverted = 1 / number;
            if (TryFormatEdgeCase(number, out var formatInverted)) return $"1/{formatInverted}";

            if (Math.Abs(inverted) > (double)decimal.MaxValue)
            {
                return $"1/{inverted:0.#e+0}";
            }

            return $"1/{Math.Round((decimal)inverted, 1):#.#}";
        }

        if (Math.Abs(number) > (double)decimal.MaxValue)
        {
            return number.ToString("0.##e+0");
        }

        return Math.Round((decimal)number, significantDigits).ToString($"#,##0.{new string(forceDecimals ? '0' : '#', significantDigits)}");
    }

    private static bool TryFormatEdgeCase(double number, out string formatNumber)
    {
        if (double.IsNaN(number))
        {
            formatNumber = "NaN";
            return true;
        }

        if (double.IsPositiveInfinity(number))
        {
            formatNumber = "inf";
            return true;
        }

        if (double.IsNegativeInfinity(number))
        {
            formatNumber = "-inf";
            return true;
        }

        formatNumber = null;
        return false;
    }
}