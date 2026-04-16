using ShortVideoCutter.Interfaces;

namespace ShortVideoCutter.Modules;

public class ModuleIO : IModuleIO
{
    private readonly bool _isEnable = true;

    public ModuleIO(bool isEnable = false)
    {
        _isEnable = isEnable;
    }

    public void InitDirectory(string directory)
    {
        if (!Directory.Exists(directory) && _isEnable)
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void DeleteFile(string outputPath)
    {
        if (FileExists(outputPath) && _isEnable)
        {
            File.Delete(outputPath);
        }
    }

    public void FileWriteAllText(string path, string content)
    {   
        if (_isEnable)
        {
            File.WriteAllText(path, content);
        }
    }

    public bool FileExists(string outputPath)
    {
        return File.Exists(outputPath) && _isEnable;
    }

    public string[] GetAllTextLines(string path)
    {
        return File.ReadAllLines(path);
    }
}