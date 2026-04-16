using NReco.VideoConverter;
using ShortVideoCutter.Interfaces;

namespace ShortVideoCutter.Modules;

public class FFMpegModule : IFFMpegModule
{
    public void TrimedVideo(string inputFile, string outputFile, float startTime, float duration)
    {
        if (StaticDI.ModuleIO.FileExists(outputFile))
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

    public void MergeMoments(string[] inputFiles, string outputFile)
    {
        var ffmpeg = new FFMpegConverter();

        ffmpeg.ConcatMedia(inputFiles, outputFile, Format.mp4, new ConcatSettings()
        {
            ConcatAudioStream = true,
            ConcatVideoStream = true,
        });
    }
}
