using ShortVideoCutter.Models;

namespace ShortVideoCutter.Interfaces;

public interface IClicker : IService
{
    public Task InitDownloadLinks(List<Season> seasons);

    public Task InitDownloadLink(Season season);

    public Task TestRequestClick();
}
