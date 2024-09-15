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
    /// <summary>
    /// The output value for calculations.
    /// </summary>
    private double _output;
    public double Output
    {
        get => _output;
        set => this.RaiseAndSetIfChanged(ref _output, value);
    }

    /// <summary>
    /// The output End Date.
    /// </summary>
    private string _endDate;

    public string EndDate
    {
        get => _endDate;
        set => this.RaiseAndSetIfChanged(ref _endDate, value);
    }  
    
    /// <summary>
    /// The output End Date.
    /// </summary>
    private DateTime _startDate;

    public DateTime StartDate
    {
        get => _startDate;
        set => this.RaiseAndSetIfChanged(ref _startDate, value);
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
    /// The users selected mode.
    /// </summary>
    private int _selection;

    public int SelectedMode
    {
        get => _selection;
        set
        {
            this.RaiseAndSetIfChanged(ref _selection, value);
            IsValid = CheckIsValid();
            
            ModeString = SelectedMode == 3 ? "Quantity: " : "Day Supply: ";
            ModeStringFlipped = SelectedMode != 3 ? "Quantity" : "Day Supply";
            IsDaySupplyMode = SelectedMode != 3;
            
            if (!IsDaySupplyMode)
                EndDate = "";
        }
    }
    
    /// <summary>
    /// The users selected mode.
    /// </summary>
    private bool _mode_s;

    public bool IsDaySupplyMode
    {
        get => _mode_s;
        set => this.RaiseAndSetIfChanged(ref _mode_s, value);
    }

    /// <summary>
    /// The users mode converted to a string.
    /// </summary>
    private string _modeString;

    public string ModeString
    {
        get => _modeString;
        set => this.RaiseAndSetIfChanged(ref _modeString, value);
    }
    
    /// <summary>
    /// The users mode converted to a string.
    /// </summary>
    private string _modeStringFlipped;

    public string ModeStringFlipped
    {
        get => _modeStringFlipped;
        set => this.RaiseAndSetIfChanged(ref _modeStringFlipped, value);
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

        ModeString = "Day Supply: ";
        ModeStringFlipped = "Quantity";
        IsDaySupplyMode = true;
        
        StartDate = DateTime.Today;
        EndDate = StartDate.ToShortDateString();
        
        CalculateCommand = ReactiveCommand.Create(DoCalculate, isInputValid);
        DoCalculate();
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

        DoCalculate();
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
            Output = 0;
        
        return false;
    }

    /// <summary>
    /// Button binding to begin calculation.
    /// </summary>
    private void DoCalculate()
    {
        Calculator.CalculatorOutput calculatorOutput = Calculator.Calculate(DaySupplyOrQuantity, Dosage, XPerWeek, SelectedMode, _startDate);

        if (calculatorOutput.Value == -1.0)
            return;
        
        Output = calculatorOutput.Value;
        EndDate = calculatorOutput.DateValue.ToShortDateString();
        
        if (!IsDaySupplyMode)
            EndDate = "";
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
}