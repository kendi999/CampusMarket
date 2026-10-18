using CampusMarket.Models;
using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class AccueilPage : ContentPage
{
    private readonly AccueilViewModel _viewModel;

    public AccueilPage(AccueilViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Rafraichir();
    }

    private async void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not Annonce annonce)
            return;

        ((CollectionView)sender!).SelectedItem = null;

        await Shell.Current.GoToAsync("detail",
            new Dictionary<string, object> { { "Annonce", annonce } });
    }
}