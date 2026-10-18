using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class InscriptionPage : ContentPage
{
    public InscriptionPage(InscriptionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}