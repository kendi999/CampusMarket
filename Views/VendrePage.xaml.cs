using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class VendrePage : ContentPage
{
    public VendrePage(VendreViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}