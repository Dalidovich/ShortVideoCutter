namespace ShortVideoCutter.Interfaces;

public interface IModuleIO
{
    public void InitDirectory(string directory);

    public void FileWriteAllText(string path, string content);

    public bool FileExists(string outputPath);

    public void DeleteFile(string outputPath);

    public string[] GetAllTextLines(string path);
}
