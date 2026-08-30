using ShortVideoCutter.DI;
using ShortVideoCutter.Interfaces.ModuleInterfaces;
using ShortVideoCutter.Modules;

namespace ShortVideoCutter;

public class Program
{
    static async Task Main(string[] args)
    {
        var textData = @"D:\MomentLibrary\TT data.txt";
        var saveDirectory = @"D:\MomentLibrary\createAndDounload";
        var exportDir = @"D:\MomentLibrary\exportedMoments";

        DIOwner.DefaultRegistrate();

        await Work(textData, saveDirectory);

        ExportVideo(exportDir, saveDirectory);
    }

    public static void ExportVideo(string saveDirectory, string exportDir)
    {
        var exporter = DIOwner.DI.GetService<IExporter>();
        var momentsCount = exporter.ExportAllMoment(saveDirectory, exportDir);
        Console.WriteLine($"Export {momentsCount} moments");
    }

    public static async Task TrainClicker()
    {
        var clicker = DIOwner.DI.GetService<IClicker>();
        await clicker.TestRequestClick();
    }

    public static async Task Work(string textData, string saveDirectory)
    {
        var statistic = DIOwner.DI.GetService<IStatistic>();

        // time for open and focus new browser tab
        await statistic.DelayBeforeStart(100);

        var data = File.ReadAllText(textData);

        var mapper = DIOwner.DI.GetService<IMapper>();
        var clicker = DIOwner.DI.GetService<IClicker>();
        var converter = DIOwner.DI.GetService<IConverterVideoProcessor>();

        var seasons = mapper.Init(data, saveDirectory);

        mapper.Check(seasons);

        await clicker.InitDownloadLinks(seasons);
        converter.Processed(seasons, saveDirectory);

        statistic.CreateStatistic(saveDirectory);
    }
}
