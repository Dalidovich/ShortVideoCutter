using ShortVideoCutter.Modules;

namespace ShortVideoCutter;

public class Program
{
    static async Task Main(string[] args)
    {
        // time for open and focus new browser tab
        await Task.Delay(5000);
        var data = File.ReadAllText(@"path to txt file");
        var saveDirectory = @"C:\Users\pops\Downloads\testDownloadVideo";
        StaticDI.Create(new Clicker(), new(), new Downloader(true), new(), new ModuleIO(true), new FFMpegModule());
        var seasons = StaticDI.Mapper.Init(data, saveDirectory);
        await StaticDI.Clicker.InitDownloadLinks(seasons);
        StaticDI.Converter.Processed(seasons, saveDirectory);
    }
}
