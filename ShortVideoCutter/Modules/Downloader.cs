using ShortVideoCutter.Interfaces;

namespace ShortVideoCutter.Modules;

public class Downloader : IDownloader
{
    private readonly IModuleIO _moduleIO;

    public Downloader(IModuleIO moduleIO)
    {
        _moduleIO = moduleIO;
    }

    public async Task Load(string url, string outputPath)
    {
        if (_moduleIO.FileExists(outputPath))
        {
            return;
        }

        using (HttpClient client = new HttpClient())
        {
            try
            {
                Console.WriteLine($"");
                Console.WriteLine($"Start {outputPath}");
                Console.WriteLine($"link {url}");
                Console.WriteLine("Start download...");

                long totalBytes = (await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    .Content.Headers.ContentLength ?? 0;

                using (var stream = await client.GetStreamAsync(url))
                using (var fileStream = new FileStream(outputPath, FileMode.Create))
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    long totalRead = 0;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;

                        Console.Write($"\rProgress: {totalRead * 100 / totalBytes}%");
                    }
                }

                Console.WriteLine("\nSuccess!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _moduleIO.DeleteFile(outputPath);
            }
        }
    }
}