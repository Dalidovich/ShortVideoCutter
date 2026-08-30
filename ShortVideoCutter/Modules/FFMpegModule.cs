using NReco.VideoConverter;
using ShortVideoCutter.Interfaces.ModuleInterfaces;

namespace ShortVideoCutter.Modules;

public class FFMpegModule : IFFMpegModule
{
    private readonly IModuleIO _moduleIO;

    public FFMpegModule(IModuleIO moduleIO)
    {
        _moduleIO = moduleIO;
    }

    public void TrimedVideo(string inputFile, string outputFile, float startTime, float duration)
    {
        if (_moduleIO.FileExists(outputFile))
        {
            Console.WriteLine($"Moment {Path.GetFileName(outputFile)} alredy exist");
            return;
        }

        Console.WriteLine($"Moment {Path.GetFileName(outputFile)} start trimed");
        var ffmpeg = new FFMpegConverter();

        ffmpeg.ConvertMedia(inputFile, Format.mp4, outputFile, Format.mp4, new ConvertSettings
        {
            Seek = startTime,
            MaxDuration = duration
        });
        Console.WriteLine($"Moment {Path.GetFileName(outputFile)} end trim");
    }

    public void MergeMoments(string[] inputFiles, string outputFileDir)
    {
        var ffmpeg = new FFMpegConverter();
        var fileName = string.Join("_", outputFileDir.Split(Path.DirectorySeparatorChar).TakeLast(3).Take(2));

        ffmpeg.ConcatMedia(inputFiles, Path.Combine(outputFileDir, $"{fileName}.mp4"), Format.mp4, new ConcatSettings()
        {
            ConcatAudioStream = true,
            ConcatVideoStream = true,
        });
    }
}
