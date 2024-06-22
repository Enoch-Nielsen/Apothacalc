using System;

namespace Apothacalc.Models;

public static class Calculator
{
    public const double DAY_SUPPLY_MIN = 1.0;
    public const double DAY_SUPPLY_MAX = 2000.0;
    
    public const double DOSE_MIN = 0.001;
    public const double DOSE_MAX = 10000;
    
    public const double PER_WEEK_MIN = 1;
    public const double PER_WEEK_MAX = 100;
        
    /// <summary>
    /// Returns the calculated value of either the Quantity or Day Supply depending on the given input
    /// and mode.
    /// </summary>
    /// <param name="daySupplyOrQuantity"></param>
    /// <param name="dosage"></param>
    /// <param name="xPerWeek"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static double Calculate(string daySupplyOrQuantity, string dosage, string xPerWeek, int mode)
    {
        if (!float.TryParse(daySupplyOrQuantity, out float parseDs)) return 0;
        if (!float.TryParse(dosage, out float parseDose)) return 0;
        if (!float.TryParse(xPerWeek, out float parseX)) return 0;

        if (mode == 0) return (int)((parseDs / (parseDose * parseX)) * 7f);

        return (Math.Round((parseDs / 7) * (parseDose * parseX) * 10) / 10.0);
    }
}