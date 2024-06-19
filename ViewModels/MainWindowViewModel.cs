
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive;
using System.Runtime.CompilerServices;
using Apothacalc.Models;
using ReactiveUI;

namespace Apothacalc.ViewModels;

using System.Threading.Tasks;

public class MainWindowViewModel : ViewModelBase
{
    private double _output;
    public double Output
    {
        get => _output;
        set => this.RaiseAndSetIfChanged(ref _output, value);
    }

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

    private int _selection;

    public int SelectedMode
    {
        get => _selection;
        set
        {
            this.RaiseAndSetIfChanged(ref _selection, value);
            IsValid = CheckIsValid();
        }
    }

    private bool _isValid;
    public bool IsValid
    {
        get => _isValid;
        set => this.RaiseAndSetIfChanged(ref _isValid, value);
    }

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

        CalculateCommand = ReactiveCommand.Create(DoCalculate, isInputValid);
    }

    private bool CheckIsValid()
    {
        // Log that the fields are empty.
        if (string.IsNullOrWhiteSpace(DaySupplyOrQuantity))            return LogData("Day supply or quanity is empty.");
        if (string.IsNullOrWhiteSpace(Dosage))                         return LogData("Dosage is empty.");
        if (string.IsNullOrWhiteSpace(XPerWeek))                       return LogData("X' per week is empty.");
 
        // Log a Parsing Error
        if (!double.TryParse(DaySupplyOrQuantity, out double parseDs)) return LogData("Day supply or quantity is not a decimal.");
        if (!double.TryParse(Dosage, out double parseDose))            return LogData("Dosage is not a decimal.");
        if (!double.TryParse(XPerWeek, out double parseX))             return LogData("X' per week is not a decimal.");

        // Log out of bounds.
        if (parseDs is > 1200.0 or < 0.1)                              return LogData("Day supply or quantity out of range.");
        if (parseDose is > 10000.0 or < 0.01)                          return LogData("Dosage out of range.");
        if (parseX is > 60.0 or < 1.0)                                 return LogData("X' per week out of range.");

        LogData("");
        return true;
    }

    private bool LogData(string error)
    {
        Log = error;
        return false;
    }

    private void DoCalculate() => Output = Calculator.Calculate(DaySupplyOrQuantity, Dosage, XPerWeek, SelectedMode);
}