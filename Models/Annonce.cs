using System.Globalization;

namespace CampusMarket.Models;

public class Annonce
{
    public int Id { get; set; }
    public string Titre { get; set; } = "";
    public decimal Prix { get; set; }
    public string Categorie { get; set; } = "";
    public string Etat { get; set; } = "";
    public string Lieu { get; set; } = "";
    public string DateTexte { get; set; } = "";
    public string Description { get; set; } = "";
    public string Image { get; set; } = "";
    public Utilisateur Vendeur { get; set; } = new();

    // Ici c'est le chemin complet du fichier photo et vide s'il n'y en a pas
    public string PhotoChemin { get; set; } = "";

    public bool APhoto => !string.IsNullOrEmpty(PhotoChemin) && File.Exists(PhotoChemin);
    public bool SansPhoto => !APhoto;

    public string PrixAffiche =>
        $"{Prix.ToString("N0", CultureInfo.GetCultureInfo("fr-FR"))} FCFA";

    public string InfoAffichee => $"{DateTexte} - {Lieu}";

    public string EtatLieu => $"{Etat} - {Lieu}";

   

    // Image de repli selon la catégorie
    public string ImageCategorie => Categorie switch
    {
        "Livres" => "cat_livres.jpg",
        "Sport" => "cat_sport.jpg",
        "Informatique" or "Electronique" => "cat_informatique.jpg",
        _ => "cat_autre.jpg"
    };

    // Images des annonces de demo
    public string ImageDemo => Id switch
    {
        1 => "produit_maths.jpg",
        2 => "produit_laptop.jpg",
        3 => "produit_blouse.jpg",
        4 => "produit_matelas.jpg",
        5 => "produit_casio.jpg",
        6 => "produit_calculatrice.jpg",
        7 => "produit_ballon.jpg",
        _ => ""
    };

    // choisir une image de demo
    public string ImageAffichee =>
        APhoto ? PhotoChemin
        : !string.IsNullOrEmpty(ImageDemo) ? ImageDemo
        : ImageCategorie;
}