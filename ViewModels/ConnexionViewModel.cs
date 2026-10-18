using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class ConnexionViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ConnexionViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErreurGenerale))]
    private bool erreurEmail;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErreurGenerale))]
    private bool erreurMotDePasse;

    [ObservableProperty]
    private string email = "";

    [ObservableProperty]
    private string motDePasse = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MessageVisible))]
    private string messageConnexion = "";

    public bool ErreurGenerale => ErreurEmail && ErreurMotDePasse;
    public bool MessageVisible => !string.IsNullOrEmpty(MessageConnexion);

    partial void OnEmailChanged(string value)
    {
        ErreurEmail = false;
        MessageConnexion = "";
    }

    partial void OnMotDePasseChanged(string value)
    {
        ErreurMotDePasse = false;
        MessageConnexion = "";
    }

    [RelayCommand]
    private async Task SeConnecter()
    {
        ErreurEmail = string.IsNullOrWhiteSpace(Email);
        ErreurMotDePasse = string.IsNullOrWhiteSpace(MotDePasse);

        if (ErreurEmail || ErreurMotDePasse)
            return;

        var utilisateur = _db.Authentifier(Email, MotDePasse);
        if (utilisateur == null)
        {
            MessageConnexion = "E-mail ou mot de passe incorrect";
            await Shell.Current.DisplayAlert("Diagnostic",
                _db.Diagnostic(Email, MotDePasse), "OK");
            return;
        }

        DonneesFictives.UtilisateurCourant = utilisateur;

        Email = "";
        MotDePasse = "";

        await Shell.Current.GoToAsync("//principal/accueil");
    }

    [RelayCommand]
    private async Task CreerCompte()
        => await Shell.Current.GoToAsync("//inscription");
}