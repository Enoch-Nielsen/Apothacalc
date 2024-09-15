using System;
using Avalonia.Controls;

namespace Apothacalc.Models;

public static class Calculator
{
    public const double DAY_SUPPLY_MIN = 1.0;
    public const double DAY_SUPPLY_MAX = 10000.0;

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
    /// <param name="endDate"></param>
    /// <returns></returns>
    public static CalculatorOutput Calculate(string daySupplyOrQuantity, string dosage, string xPerWeek, int mode, DateTime startDate)
    {
        if (!float.TryParse(daySupplyOrQuantity, out float parseDs)) return new CalculatorOutput();
        if (!float.TryParse(dosage, out float parseDose)) return new CalculatorOutput();
        if (!float.TryParse(xPerWeek, out float parseX)) return new CalculatorOutput();

        CalculatorOutput output = new();
        DateTime endDate = startDate;
        output.DateValue = endDate;

        if (mode == 3)
        {
            output.Value = (Math.Round((parseDs / 7) * (parseDose * parseX) * 10) / 10.0);
        }
        else
        {
            output.Value = (int)((parseDs / (parseDose * parseX)) * 7f);

            endDate = mode switch
            {
                0 => endDate.AddDays(output.Value),
                1 => endDate.AddDays(output.Value * 0.8),
                2 => endDate.AddDays(Math.Max(output.Value - 7, 0)),
                _ => endDate
            };
        }

        output.DateValue = endDate;

        return output;
    }

    public struct CalculatorOutput
    {
        public double Value;
        public DateTime DateValue;

        public CalculatorOutput()
        {
            Value = -1.0;
        }
    }
}