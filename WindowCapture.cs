using System;
using System.Drawing;
using System.Runtime.InteropServices;

static class WindowCapture
{
    [DllImport("user32.dll")]
    private static extern bool PrintWindow(
        IntPtr hWnd,
        IntPtr hdcBlt,
        uint nFlags
    );

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(
        IntPtr hWnd,
        out RECT rect
    );

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static Bitmap Capture(IntPtr hWnd)
    {
        GetWindowRect(
            hWnd,
            out RECT rect
        );

        int width = rect.Right - rect.Left;
        int height = rect.Bottom - rect.Top;

        Bitmap bitmap = new Bitmap(
            width,
            height
        );

        using Graphics graphics =
            Graphics.FromImage(bitmap);

        IntPtr hdc = graphics.GetHdc();

        try
        {
            PrintWindow(
                hWnd,
                hdc,
                0
            );
        }
        finally
        {
            graphics.ReleaseHdc(hdc);
        }

        return bitmap;
    }
}