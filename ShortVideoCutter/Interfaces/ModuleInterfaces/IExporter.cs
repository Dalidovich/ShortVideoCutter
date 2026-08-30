using ShortVideoCutter.Models;

namespace ShortVideoCutter.Interfaces.ModuleInterfaces;

public interface IExporter : IService
{
    public int ExportAllMoment(string exportDir, string sourceDir);
}
