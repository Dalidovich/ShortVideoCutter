using ShortVideoCutter.DI;
using ShortVideoCutter.Interfaces;
using ShortVideoCutter.Modules;

namespace ShortVideoCutter;

public class Program
{
    static async Task Main(string[] args)
    {
        // time for open and focus new browser tab
        await Task.Delay(5000);

        DIOwner.DefaultRegistrate();

        var data = File.ReadAllText(@"path to txt file");
        var saveDirectory = @"save dir";

        var mapper = DIOwner.DI.GetService<IMapper>();
        var clicker = DIOwner.DI.GetService<IClicker>();
        var converter = DIOwner.DI.GetService<IConverterVideoProcessor>();

        var seasons = mapper.Init(data, saveDirectory);
        await clicker.InitDownloadLinks(seasons);
        converter.Processed(seasons, saveDirectory);
    }
}
