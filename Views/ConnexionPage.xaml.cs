using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class ConnexionPage : ContentPage
{
    public ConnexionPage(ConnexionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}