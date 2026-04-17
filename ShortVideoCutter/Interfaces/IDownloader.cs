namespace ShortVideoCutter.Interfaces;

public interface IDownloader : IService
{
    public Task Load(string url, string outputPath);
}
