using System.Runtime.InteropServices;

namespace ShortVideoCutter.DeviceImitations;

internal class InputDeviceImitationManager
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, nuint dwExtraInfo);

    public static void Enter() => Click(ButtonsCnst.VK_RETURN);

    public static void Paste() => CompositeClick(ButtonsCnst.VK_CONTROL, ButtonsCnst.VK_V);

    public static void Copy() => CompositeClick(ButtonsCnst.VK_CONTROL, ButtonsCnst.VK_C);

    public static void SelectAll() => CompositeClick(ButtonsCnst.VK_CONTROL, ButtonsCnst.VK_A);

    public static void MouseClick((int x, int y)? point = null, bool rightMouseButton = false)
    {
        if (point.HasValue)
        {
            SetCursorPos(point.Value.x, point.Value.y);
        }
        if (rightMouseButton)
        {
            mouse_event(ButtonsCnst.MOUSEEVENTF_RIGHTDOWN | ButtonsCnst.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }
        else
        {
            mouse_event(ButtonsCnst.MOUSEEVENTF_LEFTDOWN | ButtonsCnst.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

    }

    public static void Click(byte VK_KEYCODE)
    {
        try
        {
            keybd_event(VK_KEYCODE, 0, ButtonsCnst.KEYEVENTF_KEYDOWN, nuint.Zero);
            keybd_event(VK_KEYCODE, 0, ButtonsCnst.KEYEVENTF_KEYUP, nuint.Zero);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex}");
        }
    }

    public static void CompositeClick(byte VK_KEYCODE_MOD, byte VK_KEYCODE)
    {
        try
        {
            keybd_event(VK_KEYCODE_MOD, 0, ButtonsCnst.KEYEVENTF_KEYDOWN, nuint.Zero);
            keybd_event(VK_KEYCODE, 0, ButtonsCnst.KEYEVENTF_KEYDOWN, nuint.Zero);
            keybd_event(VK_KEYCODE, 0, ButtonsCnst.KEYEVENTF_KEYUP, nuint.Zero);
            keybd_event(VK_KEYCODE_MOD, 0, ButtonsCnst.KEYEVENTF_KEYUP, nuint.Zero);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex}");
        }
    }

    public static void CompositeClick(byte VK_KEYCODE_MOD, byte VK_KEYCODE_MOD2, byte VK_KEYCODE)
    {
        try
        {
            keybd_event(VK_KEYCODE_MOD, 0, ButtonsCnst.KEYEVENTF_KEYDOWN, nuint.Zero);
            keybd_event(VK_KEYCODE_MOD2, 0, ButtonsCnst.KEYEVENTF_KEYDOWN, nuint.Zero);
            keybd_event(VK_KEYCODE, 0, ButtonsCnst.KEYEVENTF_KEYDOWN, nuint.Zero);
            keybd_event(VK_KEYCODE, 0, ButtonsCnst.KEYEVENTF_KEYUP, nuint.Zero);
            keybd_event(VK_KEYCODE_MOD2, 0, ButtonsCnst.KEYEVENTF_KEYUP, nuint.Zero);
            keybd_event(VK_KEYCODE_MOD, 0, ButtonsCnst.KEYEVENTF_KEYUP, nuint.Zero);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"error: {ex}");
        }
    }
}
