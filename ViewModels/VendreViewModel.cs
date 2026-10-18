using System.Globalization;
using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;

namespace CampusMarket.ViewModels;

public partial class VendreViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public VendreViewModel(DatabaseService db)
    {
        _db = db;
    }

    public List<string> Categories { get; } = new()
    {
        "Livres", "Electronique", "Informatique", "Sport", "Autres"
    };

    [ObservableProperty] private string titre = "";
    [ObservableProperty] private string? categorieChoisie;
    [ObservableProperty] private string prixTexte = "";
    [ObservableProperty] private string etatChoisi = "";
    [ObservableProperty] private string details = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(APhotoChoisie))]
    [NotifyPropertyChangedFor(nameof(SansPhotoChoisie))]
    private string photoChemin = "";

    public bool APhotoChoisie => !string.IsNullOrEmpty(PhotoChemin);
    public bool SansPhotoChoisie => !APhotoChoisie;

    [ObservableProperty] private bool erreurTitre;
    [ObservableProperty] private bool erreurCategorie;
    [ObservableProperty] private bool erreurPrix;
    [ObservableProperty] private bool erreurEtat;

    partial void OnTitreChanged(string value) => ErreurTitre = false;
    partial void OnCategorieChoisieChanged(string? value) => ErreurCategorie = false;
    partial void OnPrixTexteChanged(string value) => ErreurPrix = false;
    partial void OnEtatChoisiChanged(string value) => ErreurEtat = false;

    [RelayCommand]
    private void ChoisirEtat(string etat) => EtatChoisi = etat;

    [RelayCommand]
    private async Task AjouterPhoto()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null)
                return;   // l'utilisateur a annulé

            string extension = Path.GetExtension(photo.FileName);
            if (string.IsNullOrEmpty(extension))
                extension = ".jpg";

            string nom = $"photo_{Guid.NewGuid():N}{extension}";
            string destination = Path.Combine(FileSystem.AppDataDirectory, nom);

            using var source = await photo.OpenReadAsync();
            using var cible = File.Create(destination);
            await source.CopyToAsync(cible);

            PhotoChemin = destination;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Photo",
                "Impossible de charger la photo : " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task Annuler()
    {
        Reinitialiser();
        await Shell.Current.GoToAsync("//principal/accueil");
    }

    [RelayCommand]
    private async Task Publier()
    {
        ErreurTitre = string.IsNullOrWhiteSpace(Titre);
        ErreurCategorie = string.IsNullOrEmpty(CategorieChoisie);
        ErreurEtat = string.IsNullOrEmpty(EtatChoisi);

        decimal prix = 0;
        ErreurPrix = !decimal.TryParse(PrixTexte.Replace(',', '.'),
                         NumberStyles.Number, CultureInfo.InvariantCulture, out prix)
                     || prix <= 0;

        if (ErreurTitre || ErreurCategorie || ErreurPrix || ErreurEtat)
            return;

        int nouvelId = DonneesFictives.Annonces.Max(a => a.Id) + 1;

        var annonce = new Annonce
        {
            Id = nouvelId,
            Titre = Titre.Trim(),
            Prix = prix,
            Categorie = CategorieChoisie!,
            Etat = EtatChoisi,
            Lieu = "Campus",
            DateTexte = "À l'instant",
            Description = string.IsNullOrWhiteSpace(Details)
                ? "Aucune description."
                : Details.Trim(),
            Vendeur = DonneesFictives.UtilisateurCourant,
            PhotoChemin = PhotoChemin
        };

        DonneesFictives.Annonces.Insert(0, annonce);
        _db.AjouterAnnonce(annonce);

        Reinitialiser();

        await Shell.Current.DisplayAlert("Publié",
            "Votre annonce a bien été publiée.", "OK");
        await Shell.Current.GoToAsync("//principal/accueil");
    }

    private void Reinitialiser()
    {
        Titre = "";
        CategorieChoisie = null;
        PrixTexte = "";
        EtatChoisi = "";
        Details = "";
        PhotoChemin = "";
        ErreurTitre = false;
        ErreurCategorie = false;
        ErreurPrix = false;
        ErreurEtat = false;
    }
}