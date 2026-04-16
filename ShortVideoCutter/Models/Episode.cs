namespace ShortVideoCutter.Models;

public class Episode
{
    public int EpisodeNumber { get; set; }

    public List<Moment> Moments { get; set; }

    private string _downloadLink = string.Empty;

    private string _savePath = string.Empty;

    public Episode(int episodeNumber, TimeSpan startTime, TimeSpan endTime, string note)
    {
        EpisodeNumber = episodeNumber;
        Moments = new List<Moment>()
        {
            new Moment(startTime, endTime, note),
        };
    }

    public string GetSaveName(string seasonSaveName)
    {
        return $"{seasonSaveName}_ep_{EpisodeNumber}";
    }

    public bool IsAdditionEpisode() => EpisodeNumber == -1;

    public void SetDownloadLint(string link) => _downloadLink = link;

    public string GetDownloadLink() => _downloadLink;

    public string GetSavePath() => _savePath;

    public void SetSavePath(string saveDirectory, Season season)
    {
        var dir = season.GetSeasonPath(saveDirectory);
        StaticDI.ModuleIO.InitDirectory(dir);
        _savePath = Path.Combine(dir, $"{GetSaveName(season.GetSaveName())}.mp4");
    }

    public override string ToString()
    {
        return $"num {EpisodeNumber}({Moments.Count})";
    }
}
