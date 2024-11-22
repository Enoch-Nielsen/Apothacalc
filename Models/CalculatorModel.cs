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
    public static CalculatorOutputNormal Calculate(string daySupplyOrQuantity, string dosage, string xPerWeek)
    {
        if (!float.TryParse(daySupplyOrQuantity, out float parseDs)) return new CalculatorOutputNormal();
        if (!float.TryParse(dosage, out float parseDose)) return new CalculatorOutputNormal();
        if (!float.TryParse(xPerWeek, out float parseX)) return new CalculatorOutputNormal();

        CalculatorOutputNormal output = new();


        output.Quantity = (Math.Round((parseDs / 7) * (parseDose * parseX) * 10) / 10.0);

        output.DaySupply = (int)((parseDs / (parseDose * parseX)) * 7f);
        
        return output;
    }

    public static CalculatorOutputDate CalculateDates(string daySupply, DateTime startDate)
    {
        if (!float.TryParse(daySupply, out float parseDs))
        {
            throw new InvalidCastException("Unable to cast the given input for Date Day Supply!");
        }

        CalculatorOutputDate output = new();
        
        output.NewYorkDateValue = startDate;
        output.EightyDateValue = startDate;

        output.NewYorkDateValue = output.NewYorkDateValue.AddDays(Math.Max(parseDs - 7, 0));
        output.EightyDateValue = output.EightyDateValue.AddDays(parseDs * 0.8);

        return output;
    }

    public struct CalculatorOutputNormal
    {
        public double DaySupply;
        public double Quantity;

        public CalculatorOutputNormal()
        {
            DaySupply = -1.0;
        }
    }
    
    public struct CalculatorOutputDate
    {
        public DateTime EightyDateValue;
        public DateTime NewYorkDateValue;
    }
}