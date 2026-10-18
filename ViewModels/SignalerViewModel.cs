using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class SignalerViewModel : ObservableObject
{
    [ObservableProperty] private string motifChoisi = "";
    [ObservableProperty] private string details = "";
    [ObservableProperty] private bool erreurMotif;

    // Dès qu'un motif est choisi, on efface l'erreur
    partial void OnMotifChoisiChanged(string value) => ErreurMotif = false;

    [RelayCommand]
    private async Task Annuler()
        => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task Envoyer()
    {
        if (string.IsNullOrEmpty(MotifChoisi))
        {
            ErreurMotif = true;
            return;
        }

        await Shell.Current.DisplayAlert("Merci",
            "Votre signalement a bien été envoyé.", "OK");
        await Shell.Current.GoToAsync("..");
    }
}