using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class ProfilPage : ContentPage
{
    private readonly ProfilViewModel _viewModel;

    public ProfilPage(ProfilViewModel viewModel)
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
}