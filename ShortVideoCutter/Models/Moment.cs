using ShortVideoCutter.Interfaces;
using System.Text.RegularExpressions;

namespace ShortVideoCutter.Models;

public class Moment : IModelChecker
{
    public const string InvalidTrigger = "!";

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public string Note { get; set; }

    public float SecondsDuration => (float)(EndTime.TotalSeconds - StartTime.TotalSeconds);
    public float Start => (float)StartTime.TotalSeconds;

    private MomentStatus? _status = null;

    private string _savePath = string.Empty;

    public Moment(TimeSpan startTime, TimeSpan endTime, string note)
    {
        StartTime = startTime;
        EndTime = endTime;
        Note = note;
    }

    public string GetSaveName(Season season, Episode episode)
    {
        return $"{episode.GetSaveName(season.GetSaveName())}_T_{StartTime.ToString(@"mm\-ss")}_D_{(EndTime - StartTime).ToString(@"mm\-ss")}";
    }

    public string GetSavePath(bool ignoreStatus = false)
    {
        if (ignoreStatus)
        {
            return $"{_savePath}.mp4";
        }
        return _status switch
        {
            MomentStatus.Invalid => $"{_savePath}.txt",
            _ => $"{_savePath}.mp4",
        };
    }

    internal void SetSavePath(string saveDirectory, Season season, Episode episode)
    {
        var dir = Path.Combine(saveDirectory, season.GetSaveName(), $"moments");
        _savePath = Path.Combine(dir, GetSaveName(season, episode));
    }

    public MomentStatus GetStatus()
    {
        if (_status.HasValue)
        {
            return _status.Value;
        }

        var activeStatuses = new List<MomentStatus?>()
        {
            IsInvalidMoment() ? MomentStatus.Invalid : null,
            IsPartMoment().isPart ? MomentStatus.Part : null,
            MomentStatus.Simple
        };

        _status = activeStatuses.Where(x => x.HasValue).Select(x => x.Value).FirstOrDefault();

        return _status ?? MomentStatus.Invalid;
    }

    public bool IsInvalidMoment() => Note.Contains(InvalidTrigger);

    public (bool isPart, PartMomentData data) IsPartMoment()
    {
        var partPattenr = new Regex(@"ID(\d+)PART(\d+)(?:GLOB(\d+))?");
        if (partPattenr.Match(Note) is { Groups.Count: 4 } matches)
        {
            var nums = matches.Groups.Values.Skip(1).Select(x => x.Value).ToArray();
            if (int.TryParse(nums[0], out var id) && int.TryParse(nums[1], out var partSeq))
            {
                if (int.TryParse(nums[2], out int globPart))
                {
                    return (true, new PartMomentData(id, partSeq, globPart));
                }
                return (true, new PartMomentData(id, partSeq));
            }
        }
        return (false, null);
    }

    public void RepairMoment()
    {
        Note.Replace(InvalidTrigger, "");
        _status = null;
    }

    public string GetCorrectEpisodePathOrDefault()
    {
        var pattern = new Regex(@"PATH\(([A-Za-z]:\\[^<>:""|?*\n]+|[/~\\][^\n()]*)\)");
        var match = pattern.Match(Note);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return null;
    }

    public void OverwriteTimes(IMapper mapper)
    {
        var match = new Regex(@"START\((.*?)\)").Match(Note);
        if (match.Success && mapper.ParseTimeSpan(match.Groups[1].Value) is { } startTime)
        {
            StartTime = startTime;
        }
        match = new Regex(@"END\((.*?)\)").Match(Note);
        if (match.Success && mapper.ParseTimeSpan(match.Groups[1].Value) is { } endTime)
        {
            EndTime = endTime;
        }
    }

    public string HelthCheck()
    {
        if (SecondsDuration <= 0 || Start < 0)
        {
            return $"Invalid time mark";
        }

        return null;
    }
}
