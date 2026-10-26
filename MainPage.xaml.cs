using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComboBoxBindingIssue;

// Repro for Syncfusion.Maui.Inputs 35.1.41 (.NET MAUI, .NET 10, Android + iOS):
// SfComboBox with IsVisible bound to a view-model flag that is false while ItemsSource is populated
// shows an empty (gray) drop-down after the flag becomes true. The same combo without the IsVisible binding works.
//
// Steps:
//   1. MauiProgram: builder.ConfigureSyncfusionCore();
//   2. Run; wait until the status label says "loaded".
//      Tap combo 1 -> drop-down opens empty/gray.   (BUG)
//      Tap combo 2 -> drop-down lists 1..7.          (expected)
//   3. Remove IsVisible="{Binding DatesVisible}" from combo 1 -> it works too.
public partial class MainPage : ContentPage
{
    readonly MainViewModel _vm = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Simulates an async data load (server call) performed in OnAppearing.
        await Task.Delay(500);

        _vm.Items.Clear();
        for (int i = 1; i <= 7; i++)
            _vm.Items.Add(new Item { Nights = i });

        _vm.DatesVisible = true;   // data ready -> show combo 1; its drop-down is now empty/gray
        Status.Text = "loaded – open both drop-downs";
    }
}

public class Item
{
    public int Nights { get; set; }
    public string NightsText => Nights.ToString();
}

public class MainViewModel : INotifyPropertyChanged
{
    bool _datesVisible;

    public ObservableCollection<Item> Items { get; } = [];

    public bool DatesVisible
    {
        get => _datesVisible;
        set { if (_datesVisible != value) { _datesVisible = value; OnPropertyChanged(); } }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}