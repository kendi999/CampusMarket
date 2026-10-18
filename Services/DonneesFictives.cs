using CampusMarket.Models;

namespace CampusMarket.Services;

public static class DonneesFictives
{
    public static Utilisateur UtilisateurCourant { get; set; } = new()
    {
        Id = 1,
        NomComplet = "Kendi Mak",
        Email = "kendi@gmail.com",
        Etablissement = "POLYTECH Douala - ENSPD"
    };

    private static readonly Utilisateur Isaac = new()
    {
        Id = 2,
        NomComplet = "NGWEM MAKENDI Isaac",
        Filiere = "M2 Génie Logiciel - ENSPD",
        Etablissement = "POLYTECH Douala - ENSPD"
    };

    public static List<Annonce> Annonces { get; } = new()
    {
        new() { Id = 1, Titre = "Livre de Maths L2", Prix = 3000, Categorie = "Livres",
                Etat = "Bon état", Lieu = "Bloc A", DateTexte = "Il y a 2 h",
                Description = "Livre de mathématiques L2, 850 exercices corrigés.", Vendeur = Isaac },
        new() { Id = 2, Titre = "Laptop I5 8910", Prix = 120000, Categorie = "Electronique",
                Etat = "Bon état", Lieu = "Bloc B", DateTexte = "Il y a 3 jrs",
                Description = "Laptop Core i5, idéal pour les cours d'informatique.", Vendeur = Isaac },
        new() { Id = 3, Titre = "Blouse de labo", Prix = 3500, Categorie = "Autres",
                Etat = "Neuf", Lieu = "Bloc A", DateTexte = "Il y a 1 j",
                Description = "Blouse blanche de laboratoire.", Vendeur = Isaac },
        new() { Id = 4, Titre = "Matelas 1 place", Prix = 25000, Categorie = "Autres",
                Etat = "Bon état", Lieu = "Bloc C", DateTexte = "Il y a 5 jrs",
                Description = "Matelas une place, très confortable.", Vendeur = Isaac },
        new() { Id = 5, Titre = "Calculatrice Casio FX-991", Prix = 3000, Categorie = "Informatique",
                Etat = "Très bon état", Lieu = "Bloc B", DateTexte = "Il y a 4 h",
                Description = "Utilisé un semestre, pile neuve, housse incluse.\nRemise en main propre sur le campus",
                Vendeur = Isaac },
        new() { Id = 6, Titre = "Calculatrice scientifique", Prix = 5000, Categorie = "Informatique",
                Etat = "Bon état", Lieu = "Bloc B", DateTexte = "Il y a 2 jrs",
                Description = "Calculatrice scientifique en bon état.", Vendeur = Isaac },
                new() { Id = 7, Titre = "Ballon de basket", Prix = 4000, Categorie = "Sport",
                Etat = "Usé", Lieu = "Bloc A", DateTexte = "Il y a 6 jrs",
                Description = "Ballon de basket taille 7.", Vendeur = Isaac },
    
    
        
    };

    private static readonly Utilisateur Roxane = new()
    {
        Id = 3,
        NomComplet = "D.T.B.Roxane", InitialesForcees = "DR",
        Filiere = "M1 Génie Civil - ENSPD", Etablissement = "POLYTECH Douala - ENSPD"
    };

    private static readonly Utilisateur Xingxing = new()
    {
        Id = 4,
        NomComplet = "XINGXING", InitialesForcees = "XX",
        Filiere = "L3 Informatique - ENSPD", Etablissement = "POLYTECH Douala - ENSPD"
    };

    private static readonly Utilisateur User90 = new()
    {
        Id = 5,
        NomComplet = "User90", InitialesForcees = "U90",
        Filiere = "L2 Mathématiques - ENSPD", Etablissement = "POLYTECH Douala - ENSPD"
    };

    private static readonly Utilisateur User91 = new()
    {
        Id = 6,
        NomComplet = "User91", InitialesForcees = "U91",
        Filiere = "M1 Électronique - ENSPD", Etablissement = "POLYTECH Douala - ENSPD"
    };

    private static Discussion CreerDiscussion(Utilisateur u, string info, bool favori = false) => new()
    {
        Interlocuteur = u,
        DernierMessage = "Demain matin s'il vous plaît et durant la pause de 12h.",
        InfoTexte = info,
        NonLu = true,
        Favori = favori,
        Messages = new()
        {
            new() { Texte = "Bonjour cet article est-il toujours disponible s'il vous plaît ?", Heure = "11:20", EstMoi = true },
            new() { Texte = "Oui, quand voulez-vous passer à l'achat ?", Heure = "11:21", EstMoi = false },
            new() { Texte = "Demain matin s'il vous plaît et durant la pause de 12h.", Heure = "11:23", EstMoi = true },
        }
    };

    public static List<Discussion> Discussions { get; } = new()
    {
        new Discussion
        {
            Interlocuteur = Isaac,
            AnnonceLiee = Annonces.First(a => a.Id == 5),
            DernierMessage = "Demain matin s'il vous plaît et durant la pause de 12h.",
            InfoTexte = "Il y a 6 h - Bloc B",
            NonLu = true,
            Favori = true,
            EnLigne = true,
            Messages = new()
            {
                new() { Texte = "Bonjour cet article est-il toujours disponible s'il vous plaît ?", Heure = "11:20", EstMoi = true },
                new() { Texte = "Oui cela est disponible, quand voulez-vous passer à l'achat s'il vous plaît ?", Heure = "11:21", EstMoi = false },
                new() { Texte = "Demain matin s'il vous plaît et durant la pause de 12h.", Heure = "11:23", EstMoi = true },
            }
        },
        CreerDiscussion(Roxane, "Il y a 7 h - Bloc B"),
        CreerDiscussion(Xingxing, "Il y a 8 h - Bloc A", favori: true),
        CreerDiscussion(User90, "Il y a 9 h - Bloc C"),
        CreerDiscussion(User91, "Il y a 10 h - Bloc C"),
    };

    // Trouve la discussion liée à cette annonce, ou en crée une nouvelle
        // Trouve la discussion de l'utilisateur connecté liée à cette annonce, ou en crée une
    public static Discussion TrouverOuCreerDiscussion(Annonce annonce)
    {
        var existante = Discussions.FirstOrDefault(d =>
            d.ProprietaireId == UtilisateurCourant.Id && d.AnnonceLiee?.Id == annonce.Id);
        if (existante != null)
            return existante;

        var nouvelle = new Discussion
        {
            ProprietaireId = UtilisateurCourant.Id,
            Interlocuteur = annonce.Vendeur,
            AnnonceLiee = annonce,
            DernierMessage = "Nouvelle conversation",
            InfoTexte = "À l'instant",
            NonLu = false,
            EnLigne = true
        };
        Discussions.Insert(0, nouvelle);
        return nouvelle;
    }

        // Trouve (ou crée) la discussion de l'autre personne, vue de son côté
    public static Discussion TrouverOuCreerMiroir(Discussion d)
    {
        int proprietaireMiroir = d.Interlocuteur.Id;
        int interlocuteurMiroir = d.ProprietaireId;

        var existante = Discussions.FirstOrDefault(x =>
            x.ProprietaireId == proprietaireMiroir
            && x.Interlocuteur.Id == interlocuteurMiroir
            && x.AnnonceLiee?.Id == d.AnnonceLiee?.Id);
        if (existante != null)
            return existante;

        var interlocuteur = UtilisateurCourant.Id == interlocuteurMiroir
            ? UtilisateurCourant
            : Discussions.Select(x => x.Interlocuteur)
                         .Append(UtilisateurCourant)
                         .First(u => u.Id == interlocuteurMiroir);

        var miroir = new Discussion
        {
            ProprietaireId = proprietaireMiroir,
            Interlocuteur = interlocuteur,
            AnnonceLiee = d.AnnonceLiee,
            DernierMessage = "",
            InfoTexte = "À l'instant",
            NonLu = true,
            EnLigne = true
        };
        Discussions.Insert(0, miroir);
        return miroir;
    }
}