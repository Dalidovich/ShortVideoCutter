using ShortVideoCutter.DI;
using System.Threading.Tasks;

namespace ShortVideoCutter;

public class Program
{
    static async Task Main(string[] args)
    {
        DIOwner.DefaultRegistrate();
        await Start(args);
    }

    public static async Task Start(string[] args)
    {
        var cli = new CLIEngine(args);

        if (LocalSettings.Load() is { } settings)
        {
            await cli.Work(settings.TextData, settings.SaveDirectory);
            cli.ExportVideo(settings.ExportDir, settings.SaveDirectory);
            await cli.TrainClicker();
        }
    }
}
