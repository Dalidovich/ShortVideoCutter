using ShortVideoCutter.DeviceImitations;
using ShortVideoCutter.Interfaces;
using ShortVideoCutter.Models;
using System.Threading.Tasks;

namespace ShortVideoCutter.Modules;

public class Clicker : IClicker
{
    private const int _delayTime = 1000;
    private const string _linkModifyPart = "#rewind={0}_seriya_na_{1}_minute_{2}_sekunde";
    private const string _linkModifyPartForAddition = "#rewind=Доп._seriya_na_{1}_minute_{2}_sekunde";

    private readonly IModuleIO _moduleIO;
    private readonly IDownloader _downloader;

    public Clicker(IModuleIO moduleIO, IDownloader downloader)
    {
        _moduleIO = moduleIO;
        _downloader = downloader;
    }

    public async Task InitDownloadLinks(List<Season> seasons)
    {
        foreach (var season in seasons)
        {
            await InitDownloadLink(season);
        }
    }

    public async Task TestRequestClick()
    {
        await Task.Delay(2 * _delayTime);
        OnRequestClick();
    }

    private void OnRequestClick()
    {
        // On request
        InputDeviceImitationManager.MouseClick((1228, 349), true);
    }

    public async Task InitDownloadLink(Season season)
    {
        foreach (var episode in season.Episodes)
        {
            if (_moduleIO.FileExists(episode.GetSavePath()))
            {
                Console.WriteLine($"episode {Path.GetFileName(episode.GetSavePath())} alredy exist");
                continue;
            }

            _NewWindow();
            await Task.Delay(1 * _delayTime);
            ClipboardManager.SetText(_GetModifyUrl(season.Url, episode));
            await Task.Delay(1 * _delayTime);
            InputDeviceImitationManager.Paste();
            await Task.Delay(1 * _delayTime);
            InputDeviceImitationManager.Enter();
            InputDeviceImitationManager.Click(ButtonsCnst.VK_F12);
            await Task.Delay(2 * _delayTime);
            InputDeviceImitationManager.Click(ButtonsCnst.VK_F5);
            await Task.Delay(2 * _delayTime);
            OnRequestClick();
            await Task.Delay(1 * _delayTime);
            // On open in new tab
            InputDeviceImitationManager.MouseClick((1301, 377));
            // On addres link
            InputDeviceImitationManager.MouseClick((667, 63));
            await Task.Delay(1 * _delayTime);
            InputDeviceImitationManager.Copy();
            await Task.Delay(1 * _delayTime);
            episode.SetDownloadLint(ClipboardManager.GetText());
            await Task.Delay(2 * _delayTime);
            InputDeviceImitationManager.CompositeClick(ButtonsCnst.VK_CONTROL, ButtonsCnst.VK_W);
            await Task.Delay(2 * _delayTime);
            InputDeviceImitationManager.CompositeClick(ButtonsCnst.VK_CONTROL, ButtonsCnst.VK_W);
            await Task.Delay(2 * _delayTime);

            await _downloader.Load(episode.GetDownloadLink(), episode.GetSavePath());
        }
    }

    private string _GetModifyUrl(string url, Episode episode)
    {
        var formatting = episode.IsAdditionEpisode() ? _linkModifyPartForAddition : _linkModifyPart;
        return $"{url}{string.Format(formatting, episode.EpisodeNumber, episode.Moments.First().StartTime.Minutes, episode.Moments.First().StartTime.Seconds)}";
    }

    private void _NewWindow() => InputDeviceImitationManager.CompositeClick(ButtonsCnst.VK_CONTROL, ButtonsCnst.VK_SHIFT, ButtonsCnst.VK_N);
}
