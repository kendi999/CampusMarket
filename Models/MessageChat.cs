namespace CampusMarket.Models;

public class MessageChat
{
    public string Texte { get; set; } = "";
    public string Heure { get; set; } = "";
    public bool EstMoi { get; set; }
    public bool EstAutre => !EstMoi;
}