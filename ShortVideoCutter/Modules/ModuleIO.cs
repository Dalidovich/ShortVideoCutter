using ShortVideoCutter.Interfaces.ModuleInterfaces;

namespace ShortVideoCutter.Modules;

public class ModuleIO : IModuleIO
{
    public ModuleIO()
    {
    }

    public void InitDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void DeleteFile(string outputPath)
    {
        if (FileExists(outputPath))
        {
            File.Delete(outputPath);
        }
    }

    public void FileWriteAllText(string path, string content)
    {
        File.WriteAllText(path, content);
    }

    public bool FileExists(string outputPath)
    {
        return File.Exists(outputPath);
    }

    public string[] GetAllTextLines(string path)
    {
        return File.ReadAllLines(path);
    }
}