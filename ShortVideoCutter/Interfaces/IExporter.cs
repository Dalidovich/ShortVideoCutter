using ShortVideoCutter.Models;

namespace ShortVideoCutter.Interfaces;

public interface IExporter : IService
{
    public int ExportAllMoment(string exportDir, string sourceDir);
}
