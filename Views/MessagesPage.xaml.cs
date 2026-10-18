using CampusMarket.Models;
using CampusMarket.ViewModels;

namespace CampusMarket.Views;

public partial class MessagesPage : ContentPage
{
    private readonly MessagesViewModel _viewModel;

    public MessagesPage(MessagesViewModel viewModel)
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
        if (e.CurrentSelection.FirstOrDefault() is not Discussion discussion)
            return;

        ((CollectionView)sender!).SelectedItem = null;

        await Shell.Current.GoToAsync("conversation",
            new Dictionary<string, object> { { "Discussion", discussion } });
    }
}