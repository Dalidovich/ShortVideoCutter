using ShortVideoCutter.Interfaces.ModelInterfaces;

namespace ShortVideoCutter.Models;

public class Season : IModelChecker
{
    public string EnName { get; set; } = string.Empty;

    public string RuName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public List<Episode> Episodes { get; set; }

    public Season(string ruName, string enName, string url, List<Episode> episodeMoments)
    {
        Episodes = episodeMoments;
        EnName = enName;
        RuName = ruName;
        Url = url;
    }

    public string GetSaveName()
    {
        var name = (string)EnName.Clone();
        foreach (var item in Constants.UnavailableCharactersInFileName)
        {
            name = name.Replace(item, string.Empty);
        }
        return name;
    }

    public string GetSeasonPath(string saveDirectory)
    {
        return Path.Combine(saveDirectory, GetSaveName());
    }

    public override string ToString()
    {
        return $"{EnName}({Episodes.Count})";
    }

    public string HelthCheck()
    {
        if (Episodes.Count <= 0)
        {
            return $"Episodes count is zero or negative";
        }

        return null;
    }
}
