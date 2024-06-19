using System;

namespace Apothacalc.Models;

public static class Calculator
{
    public static double Calculate(string daySupplyOrQuantity, string dosage, string xPerWeek, int mode)
    {
        if (!float.TryParse(daySupplyOrQuantity, out float parseDs)) return 0;
        if (!float.TryParse(dosage, out float parseDose)) return 0;
        if (!float.TryParse(xPerWeek, out float parseX)) return 0;

        if (mode == 0) return (int)((parseDs / (parseDose * parseX)) * 7f);

        return (Math.Round((parseDs / 7) * (parseDose * parseX) * 10) / 10.0);
    }
}