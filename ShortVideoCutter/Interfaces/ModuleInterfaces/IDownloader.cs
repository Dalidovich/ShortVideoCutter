namespace ShortVideoCutter.Interfaces.ModuleInterfaces;

public interface IDownloader : IService
{
    public Task Load(string url, string outputPath);
}
