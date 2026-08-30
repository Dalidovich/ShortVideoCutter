using ShortVideoCutter.Exceptions;
using ShortVideoCutter.Interfaces.ModuleInterfaces;

namespace ShortVideoCutter.Modules;

public class Exporter : IExporter
{
    private readonly IModuleIO _moduleIO;

    public Exporter(IModuleIO moduleIO)
    {
        _moduleIO = moduleIO;
    }
    public int ExportAllMoment(string exportDir, string sourceDir)
    {
        var videos = Directory.GetFiles(sourceDir, "*.mp4", SearchOption.AllDirectories);

        var readyMomentFiles = new List<(string folder,string video)>();
        var agreeLambdas = new List<Func<string, bool>>()
        {
            x => x.Contains($"{Path.DirectorySeparatorChar}moments{Path.DirectorySeparatorChar}"),
            x => x.Contains($"{Path.DirectorySeparatorChar}Final{Path.DirectorySeparatorChar}")
        };


        foreach (var video in videos)
        {
            if (agreeLambdas.Any(x=>x(video)))
            {
                var folder =  video.Replace(sourceDir, string.Empty).Split(Path.DirectorySeparatorChar);
                readyMomentFiles.Add((folder.FirstOrDefault(x=>!string.IsNullOrEmpty(x.Trim())),video));
            }
        }

        foreach (var video in readyMomentFiles)
        {
            _moduleIO.InitDirectory(Path.Combine(exportDir,video.folder));
            File.Copy(video.video,Path.Combine(exportDir, video.folder,Path.GetFileName(video.video)));
        }

        _check(exportDir, readyMomentFiles.Count);

        return readyMomentFiles.Count;
    }

    private void _check(string exportDir, int count)
    {
        var videos = Directory.GetFiles(exportDir, "*.mp4", SearchOption.AllDirectories);

        if (videos.Length != count)
        {
            throw new VideoCutterModuleException($"Export exist video count not match to agree count ({videos.Length} exist / {count} argee)");
        }
    }
}
