using SQLite;

namespace CampusMarket.Services;

// Ces classes représentent les tables de la base (une ligne = une ligne de table).

public class UtilisateurEntite
{
    [PrimaryKey] public int Id { get; set; }
    public string NomComplet { get; set; } = "";
    public string Email { get; set; } = "";
    public string Etablissement { get; set; } = "";
    public string Filiere { get; set; } = "";
    public string InitialesForcees { get; set; } = "";
    public string MotDePasseHash { get; set; } = "";
}

public class AnnonceEntite
{
    [PrimaryKey] public int Id { get; set; }
    public string Titre { get; set; } = "";
    public double Prix { get; set; }
    public string Categorie { get; set; } = "";
    public string Etat { get; set; } = "";
    public string Lieu { get; set; } = "";
    public string DateTexte { get; set; } = "";
    public string Description { get; set; } = "";
    public int VendeurId { get; set; }
    // Plus l'ordre est grand, plus l'annonce est en haut de la liste
    public int Ordre { get; set; }

    // Nom du fichier photo (sans le dossier), vide s'il n'y en a pas
    public string PhotoNom { get; set; } = "";
}

public class DiscussionEntite
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int InterlocuteurId { get; set; }
    public int AnnonceId { get; set; }   // 0 = pas d'annonce liée
    public string DernierMessage { get; set; } = "";
    public string InfoTexte { get; set; } = "";
    public bool NonLu { get; set; }
    public bool Favori { get; set; }
    public bool EstGroupe { get; set; }
    public bool EnLigne { get; set; }
    public int Ordre { get; set; }
    public int ProprietaireId { get; set; }
}

public class MessageEntite
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int DiscussionId { get; set; }
    public string Texte { get; set; } = "";
    public string Heure { get; set; } = "";
    public bool EstMoi { get; set; }
}