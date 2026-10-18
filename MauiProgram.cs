using Microsoft.Extensions.Logging;
using CampusMarket.ViewModels;
using CampusMarket.Views;
using CampusMarket.Services;

namespace CampusMarket;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
	builder.Services.AddTransient<ConnexionViewModel>();
	builder.Services.AddTransient<ConnexionPage>();
	builder.Services.AddTransient<InscriptionViewModel>();
	builder.Services.AddTransient<InscriptionPage>();
	builder.Services.AddTransient<AccueilViewModel>();
	builder.Services.AddTransient<AccueilPage>();
	builder.Services.AddTransient<RechercheViewModel>();
	builder.Services.AddTransient<RecherchePage>();

	builder.Services.AddTransient<DetailViewModel>();
	builder.Services.AddTransient<DetailAnnoncePage>();
	builder.Services.AddTransient<SignalerViewModel>();
	builder.Services.AddTransient<SignalerPage>();

	builder.Services.AddTransient<VendreViewModel>();
	builder.Services.AddTransient<VendrePage>();

	builder.Services.AddTransient<MessagesViewModel>();
	builder.Services.AddTransient<MessagesPage>();
	builder.Services.AddTransient<ConversationViewModel>();
	builder.Services.AddTransient<ConversationPage>();
	builder.Services.AddSingleton<DatabaseService>();

	builder.Services.AddTransient<ProfilViewModel>();
	builder.Services.AddTransient<ProfilPage>();
		return builder.Build();
	}
}
