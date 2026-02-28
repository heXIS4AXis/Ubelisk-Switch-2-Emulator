using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UbeliskUI;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
    }

    private void InterfaceBtn_Click(object? sender, RoutedEventArgs e)
    {
        InterfaceBtn.Background = Avalonia.Media.Brush.Parse("#0064DC");
        GpuBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        CpuBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        EmulatorBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
    }

    private void GpuBtn_Click(object? sender, RoutedEventArgs e)
    {
        InterfaceBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        GpuBtn.Background = Avalonia.Media.Brush.Parse("#0064DC");
        CpuBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        EmulatorBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
    }

    private void CpuBtn_Click(object? sender, RoutedEventArgs e)
    {
        InterfaceBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        GpuBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        CpuBtn.Background = Avalonia.Media.Brush.Parse("#0064DC");
        EmulatorBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
    }

    private void EmulatorBtn_Click(object? sender, RoutedEventArgs e)
    {
        InterfaceBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        GpuBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        CpuBtn.Background = Avalonia.Media.Brush.Parse("Transparent");
        EmulatorBtn.Background = Avalonia.Media.Brush.Parse("#0064DC");
    }
}