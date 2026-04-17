using FluentAssertions;
using ShortVideoCutter.Models;

namespace ShortVideoCutter.Tests;

public static class AssertData
{
    public static void AssertSeason(Season srcSeason, Season expectedSeason)
    {
        srcSeason.EnName.Should().Be(expectedSeason.EnName);
        srcSeason.RuName.Should().Be(expectedSeason.RuName);
        srcSeason.Url.Should().Be(expectedSeason.Url);
        srcSeason.Episodes.Count.Should().Be(expectedSeason.Episodes.Count);
        for (int i = 0; i < srcSeason.Episodes.Count; i++)
        {
            AssertEpisode(srcSeason.Episodes[i], expectedSeason.Episodes[i]);
        }
    }

    public static void AssertEpisode(Episode srcEpisode, Episode expectedEpisode)
    {
        srcEpisode.EpisodeNumber.Should().Be(expectedEpisode.EpisodeNumber);
        srcEpisode.Moments.Count.Should().Be(expectedEpisode.Moments.Count);
        srcEpisode.GetDownloadLink().Should().Be(expectedEpisode.GetDownloadLink());
        srcEpisode.GetSavePath().Should().Be(expectedEpisode.GetSavePath());
        for (int i = 0; i < srcEpisode.Moments.Count; i++)
        {
            AssertMoment(srcEpisode.Moments[i], expectedEpisode.Moments[i]);
        }
    }

    public static void AssertMoment(Moment moment, Moment expectedMoment)
    {
        moment.StartTime.Should().Be(expectedMoment.StartTime);
        moment.EndTime.Should().Be(expectedMoment.EndTime);
    }

    public static List<Season> GetCorrectData(string saveDirectory)
    {
        var season1 = new Season("ru1", "en1", "url1", new List<Episode>());
        var s1ep1 = new Episode(1, new TimeSpan(0, 2, 0), new TimeSpan(0, 4, 0), "!");
        s1ep1.SetLinkAndPath(season1, saveDirectory);
        var s1ep3 = new Episode(3, new TimeSpan(0, 5, 0), new TimeSpan(0, 6, 0), "ID1PART1");
        s1ep3.Moments.Add(new Moment(new TimeSpan(0, 5, 0), new TimeSpan(0, 6, 0), "ID2PART1"));
        s1ep3.SetLinkAndPath(season1, saveDirectory);
        var s1ep4 = new Episode(4, new TimeSpan(0, 12, 0), new TimeSpan(0, 14, 0), "ID2PART3");
        s1ep4.SetLinkAndPath(season1, saveDirectory);
        var s1ep6 = new Episode(6, new TimeSpan(0, 20, 0), new TimeSpan(0, 40, 0), "ID2PART2");
        s1ep6.Moments.Add(new Moment(new TimeSpan(0, 20, 0), new TimeSpan(0, 40, 0), "ID2PART2GLOB1"));
        s1ep6.SetLinkAndPath(season1, saveDirectory);
        season1.Episodes.AddRange(s1ep1, s1ep3, s1ep4, s1ep6);

        var season2 = new Season("ru2", "en2", "url2", new List<Episode>());
        var s2ep1 = new Episode(4, new TimeSpan(0, 2, 0), new TimeSpan(0, 4, 0), "!");
        s2ep1.SetLinkAndPath(season2, saveDirectory);
        var s2ep7 = new Episode(7, new TimeSpan(0, 5, 0), new TimeSpan(0, 6, 0), "ID1PART1");
        s2ep7.SetLinkAndPath(season2, saveDirectory);
        var s2ep3 = new Episode(3, new TimeSpan(0, 5, 0), new TimeSpan(0, 6, 0), "! ID2PART1");
        s2ep3.SetLinkAndPath(season2, saveDirectory);
        var s2ep5 = new Episode(5, new TimeSpan(0, 12, 0), new TimeSpan(0, 14, 0), "ID2PART1GLOB2");
        s2ep5.SetLinkAndPath(season2, saveDirectory);
        var s2ep6 = new Episode(6, new TimeSpan(0, 20, 0), new TimeSpan(0, 40, 0), "ID1PART2");
        s2ep6.SetLinkAndPath(season2, saveDirectory);
        var s2ep12 = new Episode(12, new TimeSpan(0, 20, 0), new TimeSpan(0, 40, 0), "ID4PART2");
        s2ep12.SetLinkAndPath(season2, saveDirectory);
        season2.Episodes.AddRange(s2ep1, s2ep7, s2ep3, s2ep5, s2ep6, s2ep12);

        return [season1, season2];
    }

    internal static void SetLinkAndPath(this Episode episode, Season season, string saveDirectory)
    {
        episode.SetDownloadLint(string.Format(MockFactory.DownloadLinkEpisodeFormat, season.GetSaveName(), episode.EpisodeNumber));
        episode.SetSavePath(saveDirectory, season);
    }
}
