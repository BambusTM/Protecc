using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Protecc;

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
                
                fonts.AddFont("FontAwesomeBrands.otf", "FontAwesomeBrands");
                fonts.AddFont("FontAwesomeRegular.otf", "FontAwesomeRegular");
                fonts.AddFont("FontAwesomeSolid.otf", "FontAwesomeSolid");
            })
            .UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}