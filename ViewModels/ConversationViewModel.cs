using System.Collections.ObjectModel;
using CampusMarket.Models;
using CampusMarket.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CampusMarket.ViewModels;

// "Discussion" est le nom du paramètre envoyé par la page précédente
[QueryProperty("DiscussionCourante", "Discussion")]
public partial class ConversationViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ConversationViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AfficheProduit))]
    private Discussion discussionCourante = new();

    [ObservableProperty] private string nouveauMessage = "";

    public ObservableCollection<MessageChat> Messages { get; } = new();

    public bool AfficheProduit => DiscussionCourante.AnnonceLiee != null;

    // Appelée quand la page précédente envoie la discussion
    partial void OnDiscussionCouranteChanged(Discussion value)
    {
        Messages.Clear();
        foreach (var m in value.Messages)
            Messages.Add(m);

        if (value.NonLu)
        {
            value.NonLu = false;
            _db.SauvegarderDiscussions();
        }
    }

    [RelayCommand]
    private async Task Retour()
        => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task OuvrirMenu()
    {
        string action = DiscussionCourante.Favori
            ? "Retirer des favoris"
            : "Ajouter aux favoris";

        string? choix = await Shell.Current.DisplayActionSheet("Options", "Fermer", null, action);

        if (choix == action)
        {
            DiscussionCourante.Favori = !DiscussionCourante.Favori;
            _db.SauvegarderDiscussions();
        }
    }

    [RelayCommand]
    
    private void Envoyer()
    {
        string texte = NouveauMessage.Trim();
        if (texte.Length == 0)
            return;

        string heure = DateTime.Now.ToString("HH:mm");
        var message = new MessageChat { Texte = texte, Heure = heure, EstMoi = true };
        var copie = new MessageChat { Texte = texte, Heure = heure, EstMoi = false };

        var liste = DonneesFictives.Discussions;

        // Côté de l'autre personne : discussion miroir avec le message reçu
        var miroir = DonneesFictives.TrouverOuCreerMiroir(DiscussionCourante);
        bool miroirNouveau = miroir.Id == 0;
        miroir.Messages.Add(copie);
        miroir.DernierMessage = texte;
        miroir.InfoTexte = "À l'instant";
        miroir.NonLu = true;
        liste.Remove(miroir);
        liste.Insert(0, miroir);

        // Mon côté
        DiscussionCourante.DernierMessage = texte;
        DiscussionCourante.InfoTexte = "À l'instant";
        liste.Remove(DiscussionCourante);
        liste.Insert(0, DiscussionCourante);

        // Base de données;les discussions d'abord, puis les messages
        _db.SauvegarderDiscussions();
        _db.AjouterMessage(DiscussionCourante, message);
        if (!miroirNouveau)
            _db.AjouterMessage(miroir, copie);   // si le miroir est nouveau, il est déjà enregistré avec son message

        // Affichage
        DiscussionCourante.Messages.Add(message);
        Messages.Add(message);

        NouveauMessage = "";
    }
}