using ShortVideoCutter.Interfaces;
using System.Text;

namespace ShortVideoCutter.Modules;

public class Statistic : IStatistic
{
    public async Task DelayBeforeStart(int mileseconds)
    {
        Console.WriteLine("Prepare");
        for (int i = 0; i < mileseconds; i++)
        {
            await Task.Delay(1);
            if (i % 1000 == 0)
            {
                Console.WriteLine(mileseconds - i);
            }
        }
        Console.WriteLine("Start");
    }

    public void CreateStatistic(string saveDirectory)
    {
        var statisticFileName = "statistic.txt";
        var txtFiles = Directory.GetFiles(saveDirectory, "*.txt", SearchOption.AllDirectories);
        txtFiles = txtFiles.Where(x=>!x.Contains(statisticFileName)).ToArray();

        var countTxtFile = $"exist {txtFiles.Length} txt files";

        var content = new StringBuilder();
        content.AppendLine(countTxtFile);
        foreach (var txtFile in txtFiles)
        {
            var fileContent = File.ReadAllText(txtFile);
            content.AppendLine(txtFile);
            content.AppendLine(fileContent);
            content.AppendLine("\n");
        }
        content.AppendLine(countTxtFile);
        if (txtFiles.Length > 0)
        {
            File.WriteAllText(Path.Combine(saveDirectory, statisticFileName), content.ToString());
        }
    }
}
