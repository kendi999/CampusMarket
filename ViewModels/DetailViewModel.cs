using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

// "Annonce" est le nom du paramètre envoyé par la page précédente
[QueryProperty("AnnonceCourante", "Annonce")]
public partial class DetailViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public DetailViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty]
    private Annonce annonceCourante = new();

    [RelayCommand]
    private async Task Retour()
        => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task OuvrirSignalement()
        => await Shell.Current.GoToAsync("signaler");

    [RelayCommand]
    private async Task Contacter()
    {
        if (AnnonceCourante.Vendeur.Id == DonneesFictives.UtilisateurCourant.Id)
        {
            await Shell.Current.DisplayAlert("Information",
                "C'est votre propre annonce.", "OK");
            return;
        }

        var discussion = DonneesFictives.TrouverOuCreerDiscussion(AnnonceCourante);
        _db.SauvegarderDiscussions();

        await Shell.Current.GoToAsync("conversation",
            new Dictionary<string, object> { { "Discussion", discussion } });
    }
}