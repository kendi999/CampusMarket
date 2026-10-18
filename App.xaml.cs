using CampusMarket.Services;

namespace CampusMarket;

public partial class App : Application
{
    private readonly DatabaseService _db;

    public App(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // La base est chargée avant l'affichage du premier écran
        _db.Initialiser();
        return new Window(new AppShell());
    }
}