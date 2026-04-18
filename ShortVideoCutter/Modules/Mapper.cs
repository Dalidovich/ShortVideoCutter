using ShortVideoCutter.Interfaces;
using ShortVideoCutter.Models;
using System.Globalization;

namespace ShortVideoCutter.Modules;

public class Mapper : IMapper
{
    private readonly IModuleIO _moduleIO;

    public Mapper(IModuleIO moduleIO)
    {
        _moduleIO = moduleIO;
    }

    public List<Season> Init(string data, string saveDirectory)
    {
        var seasonsRaw = data.Split("---").Select(x => x.Trim());
        var seasons = seasonsRaw.Select(ParseSeason)
            .Where(x => x is not null)
            .Select(InspectEpisodeMoments)
            .ToList();

        return BindEpisodePaths(seasons, saveDirectory);
    }

    public List<Season> BindEpisodePaths(List<Season> seasons, string saveDirectory)
    {
        // Emit and Init directory of future episodes and moments
        foreach (var season in seasons)
        {
            foreach (var episode in season.Episodes)
            {
                episode.SetSavePath(saveDirectory, season);
                _moduleIO.InitDirectory(Path.GetDirectoryName(episode.GetSavePath()));
                foreach (var moment in episode.Moments)
                {
                    moment.SetSavePath(saveDirectory, season, episode);
                    _moduleIO.InitDirectory(Path.GetDirectoryName(moment.GetSavePath()));
                }
            }
        }

        return seasons;
    }

    public Season ParseSeason(string x)
    {
        var lines = x.Split("\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .ToList();

        if (lines.Count < 4)
        {
            return null;
        }

        var ruName = lines[0].Trim();
        var enName = lines[1].Trim();
        var url = lines[2].Trim();
        var episodes = lines.Skip(3).Select(ParseEpisode)
            .Where(x => x is not null).ToList();

        return new Season(ruName, enName, url, episodes);
    }

    public Episode ParseEpisode(string x)
    {
        var parametrs = x.Split(' ');
        if (parametrs.Length < 3)
        {
            return null;
        }

        var episodeNumber = int.Parse(parametrs[0]);
        var startTime = ParseTimeSpan(parametrs[1]);
        var endTime = ParseTimeSpan(parametrs[2]);
        var note = string.Empty;
        if (parametrs.Length >= 4)
        {
            note = string.Join(' ', parametrs.Skip(3).ToArray());
        }
        return new Episode(episodeNumber, startTime, endTime, note);
    }

    public Season InspectEpisodeMoments(Season season)
    {
        // Move moments from same episode to this episode
        var processedEpisodeNumber = new List<int>();
        for (int i = 0; i < season.Episodes.Count; i++)
        {
            var ep = season.Episodes[i];
            var episodeWithSamenumber = season.Episodes.Skip(i + 1).Where(x => x.EpisodeNumber == ep.EpisodeNumber).ToList();
            if (episodeWithSamenumber.Count != 0)
            {
                foreach (var sameEpisode in episodeWithSamenumber)
                {
                    ep.Moments.AddRange(sameEpisode.Moments);
                    season.Episodes.Remove(sameEpisode);
                }
            }
        }
        return season;
    }

    public TimeSpan ParseTimeSpan(string x)
    {
        if (TimeSpan.TryParseExact(x, "m':'ss", CultureInfo.InvariantCulture, out var timeSpan))
        {
            return timeSpan;
        }

        if (TimeSpan.TryParseExact(x, "h':'m':'ss", CultureInfo.InvariantCulture, out var timeSpanWithHour))
        {
            return timeSpanWithHour;
        }

        throw new Exception($"Invalid Date parse - '{x}'");
    }
}
