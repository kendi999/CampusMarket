using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class DetailAnnoncePage : ContentPage
{
    public DetailAnnoncePage(DetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}