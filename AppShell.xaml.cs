using CampusMarket.Views;

namespace CampusMarket;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Pages ouvertes par-dessus les onglets
        Routing.RegisterRoute("detail", typeof(DetailAnnoncePage));
        Routing.RegisterRoute("signaler", typeof(SignalerPage));
        Routing.RegisterRoute("conversation", typeof(ConversationPage));
    }
}