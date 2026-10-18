using System.Collections.ObjectModel;
using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

public partial class ProfilViewModel : ObservableObject
{
    [ObservableProperty] private string nomComplet = "";
    [ObservableProperty] private string email = "";
    [ObservableProperty] private string etablissement = "";
    [ObservableProperty] private string initiales = "";
    [ObservableProperty] private string titreMesAnnonces = "";

    public ObservableCollection<Annonce> MesAnnonces { get; } = new();

    public void Rafraichir()
    {
        var u = DonneesFictives.UtilisateurCourant;
        NomComplet = u.NomComplet;
        Email = u.Email;
        Etablissement = u.Etablissement;
        Initiales = u.Initiales;

        MesAnnonces.Clear();
        foreach (var a in DonneesFictives.Annonces.Where(a => a.Vendeur.Id == u.Id))
            MesAnnonces.Add(a);

        TitreMesAnnonces = MesAnnonces.Count switch
        {
            0 => "Aucune annonce publiée",
            1 => "Mes annonces (1)",
            _ => $"Mes annonces ({MesAnnonces.Count})"
        };
    }

    [RelayCommand]
    private async Task SeDeconnecter()
        => await Shell.Current.GoToAsync("//connexion");
}