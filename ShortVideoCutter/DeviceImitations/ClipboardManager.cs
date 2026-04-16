using System.Diagnostics;
using System.Text;

namespace ShortVideoCutter.DeviceImitations;

internal class ClipboardManager
{
    internal static void SetText(string text)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c echo {text} | clip",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            process.WaitForExit();

            Console.WriteLine($"text - |{text}| was copy to clipboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ClipboardManager SetText ex: {ex.Message}");
        }
    }

    internal static string GetText()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-Command Get-Clipboard",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ClipboardManager GetText ex: {ex.Message}");
            return string.Empty;
        }
    }
}
