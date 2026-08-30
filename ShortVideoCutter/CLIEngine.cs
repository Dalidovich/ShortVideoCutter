using ShortVideoCutter.DI;
using ShortVideoCutter.Interfaces.ModuleInterfaces;

namespace ShortVideoCutter;

public class CLIEngine
{
    private bool _work;
    private bool _export;
    private bool _clickTest;

    public CLIEngine(string[] args)
    {
        _work = args.Any(x=>x.Contains("--work"));
        _export = args.Any(x=>x.Contains("--export"));
        _clickTest = args.Any(x=>x.Contains("--click"));
    }

    public void ExportVideo(string saveDirectory, string exportDir)
    {
        if (!_export)
        {
            return;
        }

        var exporter = DIOwner.DI.GetService<IExporter>();
        var momentsCount = exporter.ExportAllMoment(saveDirectory, exportDir);
        Console.WriteLine($"Export {momentsCount} moments");
    }

    public async Task TrainClicker()
    {
        if (!_clickTest)
        {
            return;
        }

        var clicker = DIOwner.DI.GetService<IClicker>();
        await clicker.TestRequestClick();
    }

    public async Task Work(string textData, string saveDirectory)
    {
        if (!_work)
        {
            return;
        }

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
