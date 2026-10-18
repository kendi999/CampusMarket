using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class SignalerPage : ContentPage
{
    public SignalerPage(SignalerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}