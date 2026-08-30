namespace ShortVideoCutter.Interfaces.ModuleInterfaces;

public interface IFFMpegModule : IService
{
    public void TrimedVideo(string inputFile, string outputFile, float startTime, float duration);

    public void MergeMoments(string[] inputFiles, string outputFileDir);
}
