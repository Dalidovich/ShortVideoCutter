using ShortVideoCutter.DI;
using ShortVideoCutter.Interfaces;

namespace ShortVideoCutter;

public class Program
{
    static async Task Main(string[] args)
    {
        DIOwner.DefaultRegistrate();
        var statistic = DIOwner.DI.GetService<IStatistic>();

        // time for open and focus new browser tab
        await statistic.DelayBeforeStart(100);

        var data = File.ReadAllText(@"C:\Users\pops\Desktop\TT data.txt");
        var saveDirectory = @"C:\Users\pops\Downloads\testDownloadVideo";

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
