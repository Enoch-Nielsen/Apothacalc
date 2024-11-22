using System;
using System.Diagnostics;
using Apothacalc.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace Apothacalc.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Eighty_OnButtonPressed(object? sender, PointerPressedEventArgs e)
    {
        MainWindowViewModel.Instance?.SetWeekMode(MainWindowViewModel.WeekMode.EIGHTY_PERCENT);
    }
    
    private void NewYork_OnButtonPressed(object? sender, PointerPressedEventArgs e)
    {
        MainWindowViewModel.Instance?.SetWeekMode(MainWindowViewModel.WeekMode.NEW_YORK);
    }
}