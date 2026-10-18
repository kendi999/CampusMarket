namespace CampusMarket.Models;

public class Utilisateur
{
    public int Id { get; set; }
    public string NomComplet { get; set; } = "";
    public string Email { get; set; } = "";
    public string Etablissement { get; set; } = "";
    public string Filiere { get; set; } = "";

    // Pour imposer des initiales (ex. "U90"), 
    public string InitialesForcees { get; set; } = "";

    public string Initiales =>
        !string.IsNullOrEmpty(InitialesForcees)
            ? InitialesForcees
            : string.Concat(NomComplet.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                      .Take(3).Select(m => char.ToUpper(m[0])));
}