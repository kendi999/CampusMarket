using System.Collections.ObjectModel;
using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class MessagesViewModel : ObservableObject
{
    [ObservableProperty] private string recherche = "";
    [ObservableProperty] private string filtreActif = "Tous";

    public ObservableCollection<Discussion> DiscussionsAffichees { get; } = new();

    public string NomCampus => DonneesFictives.UtilisateurCourant.Etablissement;
    public string Initiales => DonneesFictives.UtilisateurCourant.Initiales;
    public MessagesViewModel()
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
        DiscussionsAffichees.Clear();

        foreach (var d in DonneesFictives.Discussions
                     .Where(x => x.ProprietaireId == DonneesFictives.UtilisateurCourant.Id))
        {
            bool filtreOk = FiltreActif switch
            {
                "Non lus" => d.NonLu,
                "Favoris" => d.Favori,
                "Groupes" => d.EstGroupe,
                _ => true
            };

            bool texteOk = string.IsNullOrWhiteSpace(Recherche)
                || d.Interlocuteur.NomComplet.Contains(Recherche.Trim(), StringComparison.OrdinalIgnoreCase)
                || d.DernierMessage.Contains(Recherche.Trim(), StringComparison.OrdinalIgnoreCase);

            if (filtreOk && texteOk)
                DiscussionsAffichees.Add(d);
        }
    }
}