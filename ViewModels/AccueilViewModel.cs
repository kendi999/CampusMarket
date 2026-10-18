using System.Collections.ObjectModel;
using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class AccueilViewModel : ObservableObject
{
    [ObservableProperty] private string recherche = "";
    [ObservableProperty] private string filtreActif = "Tous";

    public ObservableCollection<Annonce> AnnoncesAffichees { get; } = new();

    public string NomCampus => DonneesFictives.UtilisateurCourant.Etablissement;
    public string Initiales => DonneesFictives.UtilisateurCourant.Initiales;
    public AccueilViewModel()
    {
        Appliquer();
    }

    partial void OnRechercheChanged(string value) => Appliquer();
    partial void OnFiltreActifChanged(string value) => Appliquer();

    [RelayCommand]
    private void ChoisirFiltre(string filtre) => FiltreActif = filtre;
    public void Rafraichir()
    {
        OnPropertyChanged(nameof(NomCampus));
        OnPropertyChanged(nameof(Initiales));
        Appliquer();
    }
    private void Appliquer()
    {
        AnnoncesAffichees.Clear();

        foreach (var a in DonneesFictives.Annonces)
        {
            bool categorieOk = FiltreActif switch
            {
                "Tous" => true,
                "Livres" => a.Categorie == "Livres",
                "Electronique" => a.Categorie == "Electronique",
                _ => a.Categorie != "Livres" && a.Categorie != "Electronique"
            };

            bool texteOk = string.IsNullOrWhiteSpace(Recherche)
                || a.Titre.Contains(Recherche.Trim(), StringComparison.OrdinalIgnoreCase);

            if (categorieOk && texteOk)
                AnnoncesAffichees.Add(a);
        }
    }
}