using System;
using System.Linq;
using System.Reactive;
using Apothacalc.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using ReactiveUI;

namespace Apothacalc.ViewModels;

/// <summary>
/// # Author Enoch Nielsen
/// # Version 6/21/24
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    public static MainWindowViewModel? Instance { get; private set; }

    #region DateCalculation

    public enum WeekMode
    {
        EIGHTY_PERCENT,
        NEW_YORK
    }

    private WeekMode _weekMode = WeekMode.EIGHTY_PERCENT;

    private double _eightyVisible;

    public double EightyVisible
    {
        get => _eightyVisible;
        set => this.RaiseAndSetIfChanged(ref _eightyVisible, value);
    }
    
    private double _newYorkVisible;

    public double NewYorkVisible
    {
        get => _newYorkVisible;
        set => this.RaiseAndSetIfChanged(ref _newYorkVisible, value);
    }
    
    /// <summary>
    /// The 80% output End Date.
    /// </summary>
    private string _eightyEndDate;

    public string EightyEndDate
    {
        get => _eightyEndDate;
        set => this.RaiseAndSetIfChanged(ref _eightyEndDate, value);
    }  
    
    /// <summary>
    /// The New York output End Date.
    /// </summary>
    private string _newYorkEndDate;

    public string NewYorkEndDate
    {
        get => _newYorkEndDate;
        set => this.RaiseAndSetIfChanged(ref _newYorkEndDate, value);
    }  
    
    /// <summary>
    /// The output End Date.
    /// </summary>
    private DateTime _startDate;

    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            this.RaiseAndSetIfChanged(ref _startDate, value); 
            DoCalculateDate();            
        }
    }


    /// <summary>
    /// Day supply or quantity input value.
    /// </summary>
    private string _dateDaySupply;
    public string DateDaySupply
    {
        get => _dateDaySupply;
        set
        {
            this.RaiseAndSetIfChanged(ref _dateDaySupply, value);
            DoCalculateDate();
        }
    }

    #endregion
    
    #region QDaySupplyCalculation

    private bool _daySupplyOrQuantityVal;

    public bool DaySupplyOrQuantityVal
    {
        get => _daySupplyOrQuantityVal;
        set => this.RaiseAndSetIfChanged(ref _daySupplyOrQuantityVal, value);
    }
    
    private string _daySupplyOrQuantityText;

    public string DaySupplyOrQuantityText
    {
        get => _daySupplyOrQuantityText;
        set => this.RaiseAndSetIfChanged(ref _daySupplyOrQuantityText, value);
    }

    private bool _daySupplyOrQuantityFlippedVal;
    public bool DaySupplyOrQuantityFlippedVal
    {
        get => _daySupplyOrQuantityFlippedVal;
        set => this.RaiseAndSetIfChanged(ref _daySupplyOrQuantityFlippedVal, value);
    }

    private int _dsMode;

    public int DSMode
    {
        get => _dsMode;
        set
        {
            this.RaiseAndSetIfChanged(ref _dsMode, value);
            SetModeVal(value);
        }
    }

    /// <summary>
    /// The output value for calculations.
    /// </summary>
    private double _daySupplyOutput;
    public double DaySupplyOutput
    {
        get => _daySupplyOutput;
        set => this.RaiseAndSetIfChanged(ref _daySupplyOutput, value);
    }

    /// <summary>
    /// The output value for calculations.
    /// </summary>
    private double _quantityOutput;
    public double QuantityOutput
    {
        get => _quantityOutput;
        set => this.RaiseAndSetIfChanged(ref _quantityOutput, value);
    }
    
    /// <summary>
    /// Day supply or quantity input value.
    /// </summary>
    private string _ds_q;
    public string DaySupplyOrQuantity
    {
        get => _ds_q;
        set
        {
            this.RaiseAndSetIfChanged(ref _ds_q, value);
            IsValid = CheckIsValid();
        }
    }    

    /// <summary>
    /// Dosage input value.
    /// </summary>
    private string _dosage;
    public string Dosage
    {
        get => _dosage;
        set
        {
            this.RaiseAndSetIfChanged(ref _dosage, value);
            IsValid = CheckIsValid();
        }
    }

    /// <summary>
    /// X's per week input value.
    /// </summary>
    private string _xpw;
    public string XPerWeek
    {
        get => _xpw;
        set
        {
            this.RaiseAndSetIfChanged(ref _xpw, value);
            IsValid = CheckIsValid();
        }
    }
    
    /// <summary>
    /// Whether the users input is valid.
    /// </summary>
    private bool _isValid;
    public bool IsValid
    {
        get => _isValid;
        set => this.RaiseAndSetIfChanged(ref _isValid, value);
    }

    #endregion

    /// <summary>
    /// Log output value.
    /// </summary>
    private string _log;

    public string Log
    {
        get => _log;
        set => this.RaiseAndSetIfChanged(ref _log, value);
    }

    public ReactiveCommand<Unit, Unit> CalculateCommand { get; }
    
    public MainWindowViewModel()
    {
        IObservable<bool> isInputValid = this.WhenAnyValue(
            x => x.IsValid,
            x => x == true
        );
        
        StartDate = DateTime.Today;
        EightyEndDate = StartDate.ToShortDateString();
        NewYorkEndDate = StartDate.ToShortDateString();
        
        SetModeVal(0);
        
        CalculateCommand = ReactiveCommand.Create(DoCalculateNormal, isInputValid);
        DoCalculateNormal();

        EightyVisible = 1.0;
        NewYorkVisible = 0.25;

        Log = " ";
        
        Instance = this;
    }

    /// <summary>
    /// Checks if the users input is valid, then either logs an error or
    /// begins calculating when appropriate.
    /// </summary>
    /// <returns></returns>
    private bool CheckIsValid()
    {
        // Log that the fields are empty.
        if (string.IsNullOrWhiteSpace(DaySupplyOrQuantity))            return LogData("Day supply or quantity is empty.");
        if (string.IsNullOrWhiteSpace(Dosage))                         return LogData("Dosage is empty.");
        if (string.IsNullOrWhiteSpace(XPerWeek))                       return LogData("X' per week is empty.");
 
        // Log a Parsing Error
        if (!double.TryParse(DaySupplyOrQuantity, out double parseDs)) return LogData("Day supply or quantity is not a decimal.");
        if (!double.TryParse(Dosage, out double parseDose))            return LogData("Dosage is not a decimal.");
        if (!double.TryParse(XPerWeek, out double parseX))             return LogData("X' per week is not a decimal.");

        // Log out of bounds.
        if (parseDs is < Calculator.DAY_SUPPLY_MIN or > Calculator.DAY_SUPPLY_MAX) return LogData("Day supply or quantity out of range.");
        if (parseDose is < Calculator.DOSE_MIN or > Calculator.DOSE_MAX)           return LogData("Dosage out of range.");
        if (parseX is < Calculator.PER_WEEK_MIN or > Calculator.PER_WEEK_MAX)      return LogData("X' per week out of range.");

        DoCalculateNormal();
        LogData("");
        
        return true;
    }

    /// <summary>
    /// Logs errors to the user.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    private bool LogData(string error)
    {
        Log = error;

        if (error.Length > 1)
            DaySupplyOutput = 0;
        
        return false;
    }

    /// <summary>
    /// Button binding to begin calculation.
    /// </summary>
    private void DoCalculateNormal()
    {
        Calculator.CalculatorOutputNormal calculatorOutputNormal = Calculator.Calculate(DaySupplyOrQuantity, Dosage, XPerWeek);

        if (calculatorOutputNormal.DaySupply == -1.0)
            return;

        QuantityOutput = calculatorOutputNormal.Quantity;
        DaySupplyOutput = calculatorOutputNormal.DaySupply;
    }
    
    /// <summary>
    /// Button binding to begin calculation.
    /// </summary>
    private void DoCalculateDate()
    {
        try
        {
            Calculator.CalculatorOutputDate calculatorOutputDate = Calculator.CalculateDates(DateDaySupply, StartDate);
            EightyEndDate = calculatorOutputDate.EightyDateValue.ToShortDateString();
            NewYorkEndDate = calculatorOutputDate.NewYorkDateValue.ToShortDateString();
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    /// <summary>
    /// Button Binding to Change the Theme to Dark
    /// </summary>
    public void SetDarkTheme()
    {
        if (Application.Current != null) 
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
    }    
    
    /// <summary>
    /// Button Binding to Change the Theme to Dark
    /// </summary>
    public void SetLightTheme()
    {
        if (Application.Current != null) 
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
    }

    /// <summary>
    /// Sets the week mode.
    /// </summary>
    /// <param name="weekMode"></param>
    public void SetWeekMode(WeekMode weekMode)
    {
        _weekMode = weekMode;

        EightyVisible = _weekMode == WeekMode.EIGHTY_PERCENT ? 1.0 : 0.25;
        NewYorkVisible = weekMode == WeekMode.NEW_YORK ? 1.0 : 0.25;
    }

    private void SetModeVal(int mode)
    {
        DaySupplyOrQuantityVal = mode == 0;
        DaySupplyOrQuantityFlippedVal = !DaySupplyOrQuantityVal;
        DaySupplyOrQuantityText = DaySupplyOrQuantityVal ? "Quantity" : "Day Supply";
    }
}