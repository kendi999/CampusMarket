using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class InscriptionViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public InscriptionViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty] private string nomComplet = "";
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string etablissement = "";
    [ObservableProperty] private string motDePasse = "";
    [ObservableProperty] private string confirmation = "";
    [ObservableProperty] private bool cguAcceptees;

    [ObservableProperty] private bool erreurNom;
    [ObservableProperty] private bool erreurEmail;
    [ObservableProperty] private bool erreurEtablissement;
    [ObservableProperty] private bool erreurMotDePasse;
    [ObservableProperty] private bool erreurGenerale;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErreurConfirmationVisible))]
    private string erreurConfirmation = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MessageCompteVisible))]
    private string messageCompte = "";

    public bool ErreurConfirmationVisible => !string.IsNullOrEmpty(ErreurConfirmation);
    public bool MessageCompteVisible => !string.IsNullOrEmpty(MessageCompte);

    partial void OnNomCompletChanged(string value) => ErreurNom = false;
    partial void OnEtablissementChanged(string value) => ErreurEtablissement = false;
    partial void OnConfirmationChanged(string value) => ErreurConfirmation = "";

    partial void OnEmailChanged(string value)
    {
        ErreurEmail = false;
        MessageCompte = "";
    }

    partial void OnMotDePasseChanged(string value)
    {
        ErreurMotDePasse = false;
        MessageCompte = "";
    }

    partial void OnCguAccepteesChanged(bool value)
    {
        if (value) ErreurGenerale = false;
    }

    [RelayCommand]
    private async Task SInscrire()
    {
        ErreurNom = string.IsNullOrWhiteSpace(NomComplet);
        ErreurEmail = string.IsNullOrWhiteSpace(Email);
        ErreurEtablissement = string.IsNullOrWhiteSpace(Etablissement);
        ErreurMotDePasse = string.IsNullOrWhiteSpace(MotDePasse);

        if (string.IsNullOrWhiteSpace(Confirmation))
            ErreurConfirmation = "Veillez remplir ce champs pour vous s'inscrire s'il vous plaîte";
        else if (Confirmation != MotDePasse)
            ErreurConfirmation = "Les mots de passe ne sont pas identiques";
        else
            ErreurConfirmation = "";

        bool champsInvalides = ErreurNom || ErreurEmail || ErreurEtablissement
                               || ErreurMotDePasse || ErreurConfirmationVisible;

        ErreurGenerale = champsInvalides || !CguAcceptees;

        if (ErreurGenerale)
            return;

        // Règles du compte
        string adresse = Email.Trim();

        if (!EmailValide(adresse))
        {
            MessageCompte = "Adresse e-mail invalide";
            return;
        }

        if (MotDePasse.Length < 8)
        {
            MessageCompte = "Le mot de passe doit contenir au moins 8 caractères";
            return;
        }

        if (_db.EmailExiste(adresse))
        {
            MessageCompte = "Cet e-mail est déjà utilisé";
            return;
        }

        _db.CreerUtilisateur(NomComplet, adresse, Etablissement, MotDePasse);

        Reinitialiser();

        await Shell.Current.DisplayAlert("Compte créé",
            "Vous pouvez maintenant vous connecter.", "OK");
        await Shell.Current.GoToAsync("//connexion");
    }

    [RelayCommand]
    private async Task SeConnecter()
        => await Shell.Current.GoToAsync("//connexion");

    private static bool EmailValide(string e)
    {
        int arobase = e.IndexOf('@');
        return arobase > 0
               && !e.Contains(' ')
               && e.IndexOf('.', arobase) > arobase + 1
               && !e.EndsWith('.');
    }

    private void Reinitialiser()
    {
        NomComplet = "";
        Email = "";
        Etablissement = "";
        MotDePasse = "";
        Confirmation = "";
        CguAcceptees = false;
        ErreurNom = false;
        ErreurEmail = false;
        ErreurEtablissement = false;
        ErreurMotDePasse = false;
        ErreurConfirmation = "";
        ErreurGenerale = false;
        MessageCompte = "";
    }
}