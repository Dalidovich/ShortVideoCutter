using ShortVideoCutter.Interfaces;
using ShortVideoCutter.Modules;

namespace ShortVideoCutter.DI;

public static class DIOwner
{
    public static LittleDI DI { get; private set; } = new LittleDI();

    public static void DefaultRegistrate()
    {
        // TODO: Order of registrate should not be indicated by a person

        DI.AddService(typeof(IModuleIO), typeof(ModuleIO));
        DI.AddService(typeof(IMapper), typeof(Mapper));
        DI.AddService(typeof(IDownloader), typeof(Downloader));
        DI.AddService(typeof(IClicker), typeof(Clicker));
        DI.AddService(typeof(IFFMpegModule), typeof(FFMpegModule));
        DI.AddService(typeof(IConverterVideoProcessor), typeof(ConverterVideoProcessor));
    }
}
