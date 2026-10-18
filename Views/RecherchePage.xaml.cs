using CampusMarket.Models;
using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class RecherchePage : ContentPage
{
    private readonly RechercheViewModel _viewModel;

    public RecherchePage(RechercheViewModel viewModel)
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