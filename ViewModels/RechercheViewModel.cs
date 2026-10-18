using System.Collections.ObjectModel;
using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class RechercheViewModel : ObservableObject
{
    private static readonly string[] Etats = { "", "Neuf", "Très bon état", "Bon état", "Usé" };

    [ObservableProperty] private string recherche = "";
    [ObservableProperty] private string categorieActive = "";
    [ObservableProperty] private string resultatsTexte = "";

    // 0 = inactif, 1 = croissant, 2 = décroissant
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LibellePrix))]
    [NotifyPropertyChangedFor(nameof(PrixActif))]
    private int modePrix;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LibelleTri))]
    [NotifyPropertyChangedFor(nameof(TriActif))]
    private int modeTri;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LibelleEtat))]
    [NotifyPropertyChangedFor(nameof(EtatActif))]
    private int etatIndex;

    public ObservableCollection<Annonce> Resultats { get; } = new();

    public string NomCampus => DonneesFictives.UtilisateurCourant.Etablissement;
    public string Initiales => DonneesFictives.UtilisateurCourant.Initiales;
    public string LibellePrix => ModePrix switch { 1 => "Prix ↑", 2 => "Prix ↓", _ => "Prix" };
    public string LibelleTri => ModeTri switch { 1 => "A → Z", 2 => "Z → A", _ => "Trier" };
    public string LibelleEtat => EtatIndex == 0 ? "Etat" : Etats[EtatIndex];

    public bool PrixActif => ModePrix != 0;
    public bool TriActif => ModeTri != 0;
    public bool EtatActif => EtatIndex != 0;

    public RechercheViewModel()
    {
        Appliquer();
    }

    partial void OnRechercheChanged(string value) => Appliquer();
    partial void OnCategorieActiveChanged(string value) => Appliquer();

    [RelayCommand]
    private void ChoisirCategorie(string categorie)
        => CategorieActive = CategorieActive == categorie ? "" : categorie;

    [RelayCommand]
    private void CyclerPrix()
    {
        ModePrix = (ModePrix + 1) % 3;
        ModeTri = 0;
        Appliquer();
    }

    [RelayCommand]
    private void CyclerTri()
    {
        ModeTri = (ModeTri + 1) % 3;
        ModePrix = 0;
        Appliquer();
    }

    [RelayCommand]
    private void CyclerEtat()
    {
        EtatIndex = (EtatIndex + 1) % Etats.Length;
        Appliquer();
    }

    public void Rafraichir()
    {
        OnPropertyChanged(nameof(NomCampus));
        OnPropertyChanged(nameof(Initiales));
        Appliquer();
    }

    private bool CorrespondCategorie(Annonce a) => CategorieActive switch
    {
        "" => true,
        "Livres" => a.Categorie == "Livres",
        "Sport" => a.Categorie == "Sport",
        "Informatique" => a.Categorie == "Informatique" || a.Categorie == "Electronique",
        _ => a.Categorie != "Livres" && a.Categorie != "Sport"
             && a.Categorie != "Informatique" && a.Categorie != "Electronique"
    };

    private void Appliquer()
    {
        IEnumerable<Annonce> liste = DonneesFictives.Annonces.Where(a =>
            CorrespondCategorie(a)
            && (EtatIndex == 0 || a.Etat == Etats[EtatIndex])
            && (string.IsNullOrWhiteSpace(Recherche)
                || a.Titre.Contains(Recherche.Trim(), StringComparison.OrdinalIgnoreCase)));

        if (ModePrix == 1) liste = liste.OrderBy(a => a.Prix);
        else if (ModePrix == 2) liste = liste.OrderByDescending(a => a.Prix);
        else if (ModeTri == 1) liste = liste.OrderBy(a => a.Titre);
        else if (ModeTri == 2) liste = liste.OrderByDescending(a => a.Titre);

        Resultats.Clear();
        foreach (var a in liste)
            Resultats.Add(a);

        ResultatsTexte = Resultats.Count switch
        {
            0 => "Aucun résultat trouvé",
            1 => "1 résultat trouvé",
            _ => $"{Resultats.Count} résultats trouvés"
        };
    }
}