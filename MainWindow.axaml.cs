using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace UbeliskUI;

public partial class MainWindow : Window
{
    bool optionsOpen = false;
    bool fileOpen = false;

    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        try
        {
            await Task.Delay(3000);

            for (double i = 1.0; i >= 0; i -= 0.05)
            {
                IntroScreen.Opacity = i;
                await Task.Delay(16);
            }

            IntroScreen.IsVisible = false;
            MainScreen.IsVisible = true;
            MainScreen.Opacity = 0;

            for (double i = 0.0; i <= 1.0; i += 0.05)
            {
                MainScreen.Opacity = i;
                await Task.Delay(16);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void FileBtn_Click(object? sender, RoutedEventArgs e)
    {
        fileOpen = !fileOpen;
        FileDropdown.IsVisible = fileOpen;
        OptionsDropdown.IsVisible = false;
        optionsOpen = false;
    }

    private async void OpenFolder_Click(object? sender, RoutedEventArgs e)
    {
        FileDropdown.IsVisible = false;
        fileOpen = false;
        var dialog = new OpenFolderDialog();
        dialog.Title = "Select ROM Folder";
        var result = await dialog.ShowAsync(this);
        if (result != null)
        {
            StatusText.Text = $"Folder: {result}";
        }
    }

    private async void OpenFile_Click(object? sender, RoutedEventArgs e)
    {
        FileDropdown.IsVisible = false;
        fileOpen = false;
        var dialog = new OpenFileDialog();
        dialog.Title = "Load ROM";
        dialog.Filters!.Add(new FileDialogFilter { Name = "ROM Files", Extensions = { "rom", "bin", "nsp", "xci" } });
        dialog.Filters.Add(new FileDialogFilter { Name = "All Files", Extensions = { "*" } });
        var result = await dialog.ShowAsync(this);
        if (result != null && result.Length > 0)
        {
            StatusText.Text = $"Loaded: {System.IO.Path.GetFileName(result[0])}";
        }
    }

    private void OptionsBtn_Click(object? sender, RoutedEventArgs e)
    {
        optionsOpen = !optionsOpen;
        OptionsDropdown.IsVisible = optionsOpen;
        FileDropdown.IsVisible = false;
        fileOpen = false;
    }

    private void FullscreenCheck_Checked(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.FullScreen;
        OptionsDropdown.IsVisible = false;
        optionsOpen = false;
    }

    private void FullscreenCheck_Unchecked(object? sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Normal;
    }

    private void ChangeProfile_Click(object? sender, RoutedEventArgs e)
    {
        OptionsDropdown.IsVisible = false;
        optionsOpen = false;
        StatusText.Text = "Profile feature coming soon!";
    }

    private void RestartBtn_Click(object? sender, RoutedEventArgs e)
    {
        var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName;
        System.Diagnostics.Process.Start(exePath);
        this.Close();
    }

    private void OpenSettings_Click(object? sender, RoutedEventArgs e)
    {
        OptionsDropdown.IsVisible = false;
        optionsOpen = false;
        var settings = new SettingsWindow();
        settings.ShowDialog(this);
    }
}