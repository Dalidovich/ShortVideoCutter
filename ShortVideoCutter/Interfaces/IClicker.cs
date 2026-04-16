using ShortVideoCutter.Models;

namespace ShortVideoCutter.Interfaces;

public interface IClicker
{
    public Task InitDownloadLinks(List<Season> seasons);

    public Task InitDownloadLink(Season season);
}
