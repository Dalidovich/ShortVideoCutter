namespace ShortVideoCutter.Interfaces.ModuleInterfaces;

public interface IModuleIO : IService
{
    public void InitDirectory(string directory);

    public void FileWriteAllText(string path, string content);

    public bool FileExists(string outputPath);

    public void DeleteFile(string outputPath);

    public string[] GetAllTextLines(string path);
}
