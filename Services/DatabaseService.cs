using System.Security.Cryptography;
using System.Text;
using CampusMarket.Models;
using Microsoft.Maui.Storage;
using SQLite;

namespace CampusMarket.Services;

public class DatabaseService
{
    private SQLiteConnection? _db;

    private SQLiteConnection Db => _db ??= Ouvrir();

    private static SQLiteConnection Ouvrir()
    {
        string chemin = Path.Combine(FileSystem.AppDataDirectory, "campusmarket.db3");
        var db = new SQLiteConnection(chemin);
        // CreateTable ajoute aussi les nouvelles colonnes aux tables existantes
        db.CreateTable<UtilisateurEntite>();
        db.CreateTable<AnnonceEntite>();
        db.CreateTable<DiscussionEntite>();
        db.CreateTable<MessageEntite>();
        return db;
    }

    // Appelée une fois au démarrage
    public void Initialiser()
    {
        if (Db.Table<UtilisateurEntite>().Count() == 0)
            Semer();     // premier lancement : on copie les données fictives dans la base

        Migrer();        // remet à niveau les données créées avant les comptes
        Charger();       // remplit les listes en mémoire depuis la base
    }

    // ---------- Premier lancement ----------

    private void Semer()
    {
        var utilisateurs = DonneesFictives.Annonces.Select(a => a.Vendeur)
            .Concat(DonneesFictives.Discussions.Select(d => d.Interlocuteur))
            .Append(DonneesFictives.UtilisateurCourant)
            .DistinctBy(u => u.Id);

        foreach (var u in utilisateurs)
            Db.Insert(VersEntite(u));

        // À l'envers : la première annonce de la liste reçoit l'ordre le plus élevé
        foreach (var a in Enumerable.Reverse(DonneesFictives.Annonces))
            AjouterAnnonce(a);

        SauvegarderDiscussions();
    }

    private void Migrer()
    {
        // Les discussions d'avant les comptes appartiennent à Kendi (Id 1)
        Db.Execute("UPDATE DiscussionEntite SET ProprietaireId = 1 WHERE ProprietaireId IS NULL OR ProprietaireId = 0");

        // Compte de démonstration : kendi999@gmail.com / 12345678@
        var kendi = Db.Find<UtilisateurEntite>(1);
        if (kendi != null && string.IsNullOrEmpty(kendi.MotDePasseHash))
        {
            kendi.MotDePasseHash = Hacher("12345678@");
            if (string.IsNullOrEmpty(kendi.Email))
                kendi.Email = "kendi999@gmail.com";
            Db.Update(kendi);
        }
    }

    // ---------- Chargement ----------

    private void Charger()
    {
        // Utilisateurs
        var utilisateurs = new Dictionary<int, Utilisateur>();
        foreach (var e in Db.Table<UtilisateurEntite>().ToList())
        {
            utilisateurs[e.Id] = e.Id == DonneesFictives.UtilisateurCourant.Id
                ? DonneesFictives.UtilisateurCourant
                : VersModele(e);
        }

        // Annonces
        var annoncesParId = new Dictionary<int, Annonce>();
        var listeAnnonces = new List<Annonce>();
        foreach (var e in Db.Table<AnnonceEntite>().OrderByDescending(x => x.Ordre).ToList())
        {
            var annonce = new Annonce
            {
                Id = e.Id,
                Titre = e.Titre,
                Prix = (decimal)e.Prix,
                Categorie = e.Categorie,
                Etat = e.Etat,
                Lieu = e.Lieu,
                DateTexte = e.DateTexte,
                Description = e.Description,
                Vendeur = utilisateurs[e.VendeurId],
                PhotoChemin = string.IsNullOrEmpty(e.PhotoNom)
                    ? ""
                    : Path.Combine(FileSystem.AppDataDirectory, e.PhotoNom)
            };
            annoncesParId[annonce.Id] = annonce;
            listeAnnonces.Add(annonce);
        }

        // Discussions et leurs messages
        var listeDiscussions = new List<Discussion>();
        foreach (var e in Db.Table<DiscussionEntite>().OrderByDescending(x => x.Ordre).ToList())
        {
            int discussionId = e.Id;

            Annonce? liee = null;
            if (e.AnnonceId != 0)
                annoncesParId.TryGetValue(e.AnnonceId, out liee);

            var messages = Db.Table<MessageEntite>()
                .Where(m => m.DiscussionId == discussionId)
                .OrderBy(m => m.Id)
                .ToList()
                .Select(m => new MessageChat { Texte = m.Texte, Heure = m.Heure, EstMoi = m.EstMoi })
                .ToList();

            listeDiscussions.Add(new Discussion
            {
                Id = e.Id,
                ProprietaireId = e.ProprietaireId,
                Interlocuteur = utilisateurs[e.InterlocuteurId],
                AnnonceLiee = liee,
                DernierMessage = e.DernierMessage,
                InfoTexte = e.InfoTexte,
                NonLu = e.NonLu,
                Favori = e.Favori,
                EstGroupe = e.EstGroupe,
                EnLigne = e.EnLigne,
                Messages = messages
            });
        }

        DonneesFictives.Annonces.Clear();
        DonneesFictives.Annonces.AddRange(listeAnnonces);
        DonneesFictives.Discussions.Clear();
        DonneesFictives.Discussions.AddRange(listeDiscussions);
    }

    // ---------- Comptes ----------

    public bool EmailExiste(string email)
    {
        string cible = email.Trim().ToLowerInvariant();
        return Db.Table<UtilisateurEntite>().ToList()
                 .Any(u => (u.Email ?? "").Trim().ToLowerInvariant() == cible);
    }

    public Utilisateur CreerUtilisateur(string nom, string email, string etablissement, string motDePasse)
    {
        int id = Db.Table<UtilisateurEntite>().ToList().Max(u => u.Id) + 1;

        var entite = new UtilisateurEntite
        {
            Id = id,
            NomComplet = nom.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Etablissement = etablissement.Trim(),
            MotDePasseHash = Hacher(motDePasse)
        };
        Db.Insert(entite);
        return VersModele(entite);
    }

    // Renvoie l'utilisateur si l'e-mail et le mot de passe sont corrects, sinon null
    public Utilisateur? Authentifier(string email, string motDePasse)
    {
        string cible = email.Trim().ToLowerInvariant();
        string hash = Hacher(motDePasse);

        var entite = Db.Table<UtilisateurEntite>().ToList().FirstOrDefault(u =>
            (u.Email ?? "").Trim().ToLowerInvariant() == cible
            && !string.IsNullOrEmpty(u.MotDePasseHash)
            && u.MotDePasseHash == hash);

        return entite == null ? null : VersModele(entite);
    }

        // TEMPORAIRE : pour comprendre pourquoi la connexion échoue
    public string Diagnostic(string email, string motDePasse)
    {
        var sb = new StringBuilder();
        string cible = email.Trim().ToLowerInvariant();
        string hash = Hacher(motDePasse);

        sb.AppendLine($"Saisi : [{cible}] (longueur mot de passe : {motDePasse.Length})");
        foreach (var u in Db.Table<UtilisateurEntite>().ToList())
        {
            string mail = (u.Email ?? "").Trim().ToLowerInvariant();
            bool hashVide = string.IsNullOrEmpty(u.MotDePasseHash);
            sb.AppendLine($"#{u.Id} [{mail}] hash vide : {hashVide}"
                + $" | email = {mail == cible}"
                + $" | hash = {u.MotDePasseHash == hash}");
        }
        return sb.ToString();
    }

    private static string Hacher(string texte)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texte)));

    // ---------- Annonces ----------

    public void AjouterAnnonce(Annonce a)
    {
        var existantes = Db.Table<AnnonceEntite>().ToList();
        int ordre = existantes.Count == 0 ? 1 : existantes.Max(x => x.Ordre) + 1;

        Db.Insert(new AnnonceEntite
        {
            Id = a.Id,
            Titre = a.Titre,
            Prix = (double)a.Prix,
            Categorie = a.Categorie,
            Etat = a.Etat,
            Lieu = a.Lieu,
            DateTexte = a.DateTexte,
            Description = a.Description,
            VendeurId = a.Vendeur.Id,
            Ordre = ordre,
            PhotoNom = string.IsNullOrEmpty(a.PhotoChemin) ? "" : Path.GetFileName(a.PhotoChemin)
        });
    }

    // ---------- Discussions ----------

    public void SauvegarderDiscussions()
    {
        var liste = DonneesFictives.Discussions;

        Db.RunInTransaction(() =>
        {
            for (int i = 0; i < liste.Count; i++)
            {
                var d = liste[i];
                var e = new DiscussionEntite
                {
                    Id = d.Id,
                    ProprietaireId = d.ProprietaireId,
                    InterlocuteurId = d.Interlocuteur.Id,
                    AnnonceId = d.AnnonceLiee?.Id ?? 0,
                    DernierMessage = d.DernierMessage,
                    InfoTexte = d.InfoTexte,
                    NonLu = d.NonLu,
                    Favori = d.Favori,
                    EstGroupe = d.EstGroupe,
                    EnLigne = d.EnLigne,
                    Ordre = liste.Count - i
                };

                if (d.Id == 0)
                {
                    Db.Insert(e);
                    d.Id = e.Id;
                    foreach (var m in d.Messages)
                        Db.Insert(VersEntite(m, d.Id));
                }
                else
                {
                    Db.Update(e);
                }
            }
        });
    }

    public void AjouterMessage(Discussion d, MessageChat m)
        => Db.Insert(VersEntite(m, d.Id));

    // ---------- Conversions ----------

    private static Utilisateur VersModele(UtilisateurEntite e) => new()
    {
        Id = e.Id,
        NomComplet = e.NomComplet,
        Email = e.Email,
        Etablissement = e.Etablissement,
        Filiere = e.Filiere,
        InitialesForcees = e.InitialesForcees
    };

    private static UtilisateurEntite VersEntite(Utilisateur u) => new()
    {
        Id = u.Id,
        NomComplet = u.NomComplet,
        Email = u.Email,
        Etablissement = u.Etablissement,
        Filiere = u.Filiere,
        InitialesForcees = u.InitialesForcees
    };

    private static MessageEntite VersEntite(MessageChat m, int discussionId) => new()
    {
        DiscussionId = discussionId,
        Texte = m.Texte,
        Heure = m.Heure,
        EstMoi = m.EstMoi
    };
}