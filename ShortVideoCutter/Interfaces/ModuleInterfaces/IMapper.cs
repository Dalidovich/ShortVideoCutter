using ShortVideoCutter.Models;

namespace ShortVideoCutter.Interfaces.ModuleInterfaces;

public interface IMapper : IService
{
    public List<Season> Init(string data, string saveDirectory);

    public List<Season> BindEpisodePaths(List<Season> seasons, string saveDirectory);

    public Season ParseSeason(string x);

    public Episode ParseEpisode(string x);

    public Season InspectEpisodeMoments(Season season);

    public TimeSpan? ParseTimeSpan(string x);

    public void Check(List<Season> seasons);
}
