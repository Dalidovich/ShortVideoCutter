using Moq;
using ShortVideoCutter.Interfaces.ModuleInterfaces;
using ShortVideoCutter.Models;

namespace ShortVideoCutter.Tests;

public class MockFactory
{
    public static string DownloadLinkEpisodeFormat = "link_{0}_ep{1}";

    public static IModuleIO CreateModuleIO(List<string> textFiles)
    {
        var mock = new Mock<IModuleIO>();

        mock.Setup(x => x.InitDirectory(It.IsAny<string>()));
        mock.Setup(x => x.FileExists(It.IsAny<string>()))
            .Returns((string path) =>
            {
                return !path.EndsWith("txt");
            });
        mock.Setup(x => x.FileWriteAllText(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string path, string content) =>
            {
                textFiles.Add(path);
            });

        return mock.Object;
    }

    public static IFFMpegModule CreateFFMpegModule(List<string> momentFiles)
    {
        var mock = new Mock<IFFMpegModule>();

        mock.Setup(x => x.TrimedVideo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()))
            .Callback((string inputFile, string outputFile, float startTime, float duration) =>
            {
                momentFiles.Add(outputFile);
            });
        mock.Setup(x => x.MergeMoments(It.IsAny<string[]>(), It.IsAny<string>()))
            .Callback((string[] inputFiles, string outputFile) =>
            {
                momentFiles.Add(outputFile);
            });

        return mock.Object;
    }

    internal static IClicker CreateClicker()
    {
        var mock = new Mock<IClicker>();

        mock.Setup(x => x.InitDownloadLink(It.IsAny<Season>()))
            .Callback((Season season) =>
            {
                foreach (var episode in season.Episodes)
                {
                    episode.SetDownloadLint(string.Format(DownloadLinkEpisodeFormat, season.GetSaveName(), episode.EpisodeNumber));
                }
            });

        mock.Setup(x => x.InitDownloadLinks(It.IsAny<List<Season>>()))
            .Callback((List<Season> seasons) =>
            {
                foreach (var season in seasons)
                {
                    mock.Object.InitDownloadLink(season);
                }
            });

        return mock.Object;
    }
}
