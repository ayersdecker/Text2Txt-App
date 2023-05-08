using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using TesseractOcrMaui;

namespace Text2Txt;

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
			}).UseMauiCommunityToolkit();

        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        // Inject logging, (optional, but gives info)
        builder.Services.AddLogging();

        // Inject library functionality
        builder.Services.AddTesseractOcr(
            files =>
            {
                files.AddFile("eng.traineddata");
            });

        // Inject main page
        builder.Services.AddSingleton<MainPage>();
#endif

        return builder.Build();
	}
}
