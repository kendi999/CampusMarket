namespace CampusMarket.Models;

public class Discussion
{
    public int ProprietaireId { get; set; }
    public int Id { get; set; }
    public Utilisateur Interlocuteur { get; set; } = new();
    public Annonce? AnnonceLiee { get; set; }
    public string DernierMessage { get; set; } = "";
    public string InfoTexte { get; set; } = "";
    public bool NonLu { get; set; }
    public bool Favori { get; set; }
    public bool EstGroupe { get; set; }
    public bool EnLigne { get; set; }
    public List<MessageChat> Messages { get; set; } = new();
}