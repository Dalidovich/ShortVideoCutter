using ShortVideoCutter.Interfaces;
using ShortVideoCutter.Modules;

namespace ShortVideoCutter;

public static class StaticDI
{
    public static IClicker Clicker { get; private set; }
    public static ConverterVideo Converter{ get; private set; }
    public static Downloader Downloader { get; private set; }
    public static IFFMpegModule FFMpegModule { get; private set; }
    public static Mapper Mapper { get; private set; }
    public static IModuleIO ModuleIO { get; private set; }

    public static void Create(IClicker clicker, ConverterVideo converter, Downloader downloader, Mapper mapper, IModuleIO moduleIO, IFFMpegModule fFMpegModule)
    {
        Clicker = clicker;
        Converter = converter;
        Downloader = downloader;
        Mapper = mapper;
        ModuleIO = moduleIO;
        FFMpegModule = fFMpegModule;
    }
}
