using ShortVideoCutter.Interfaces;
using ShortVideoCutter.Modules;

namespace ShortVideoCutter.DI;

public static class DIOwner
{
    public static LittleDI DI { get; private set; } = new LittleDI();

    public static void DefaultRegistrate()
    {
        DI.RegistrateService<IConverterVideoProcessor, ConverterVideoProcessor>();
        DI.RegistrateService<IModuleIO, ModuleIO>();
        DI.RegistrateService<IMapper, Mapper>();
        DI.RegistrateService<IDownloader, Downloader>();
        DI.RegistrateService<IClicker, Clicker>();
        DI.RegistrateService<IFFMpegModule, FFMpegModule>();

        DI.Run();
    }
}
