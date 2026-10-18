using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class ConversationPage : ContentPage
{
    public ConversationPage(ConversationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}