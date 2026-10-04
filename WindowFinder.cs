using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

static class WindowFinder
{
    public delegate bool EnumWindowsProc(
        IntPtr hWnd,
        IntPtr lParam
    );

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool EnumWindows(
        EnumWindowsProc enumProc,
        IntPtr lParam
    );

    [System.Runtime.InteropServices.DllImport(
        "user32.dll",
        CharSet = System.Runtime.InteropServices.CharSet.Unicode
    )]
    public static extern int GetWindowText(
        IntPtr hWnd,
        StringBuilder text,
        int count
    );

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags
    );

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool IsWindowVisible(
        IntPtr hWnd
    );

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool GetWindowRect(
        IntPtr hWnd,
        out RECT rect
    );

    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

        public static IntPtr ChooseWindow()
    {
        List<IntPtr> handles = new();
        List<string> titles = new();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder title = new(256);

                GetWindowText(
                    hWnd,
                    title,
                    title.Capacity
                );

                if (title.Length > 0)
                {
                    handles.Add(hWnd);
                    titles.Add(title.ToString());
                }
            }

            return true;
        }, IntPtr.Zero);

        Console.WriteLine();
        Console.WriteLine("Choose a window to cast:");
        Console.WriteLine();

        for (int i = 0; i < titles.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {titles[i]}"
            );
        }

        Console.WriteLine();
        Console.Write("Enter number: ");

        int choice = int.Parse(
            Console.ReadLine()!
        );

        return handles[choice - 1];
    }

    public class WindowInfo
    {
        public IntPtr Handle { get; set; }

        public string Title { get; set; } = "";

        public override string ToString()
        {
            return Title;
        }
    }

        public static List<WindowInfo> GetWindows()
    {
        List<WindowInfo> windows = new();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder title = new(256);

                GetWindowText(
                    hWnd,
                    title,
                    title.Capacity
                );

                if (title.Length > 0)
                {
                    windows.Add(
                        new WindowInfo
                        {
                            Handle = hWnd,
                            Title = title.ToString()
                        }
                    );
                }
            }

            return true;
        }, IntPtr.Zero);

        return windows;
    }

    public static void MoveWindowToMonitor(
        IntPtr windowHandle,
        Rectangle monitorBounds
    )
    {
        SetWindowPos(
            windowHandle,
            IntPtr.Zero,
            monitorBounds.X,
            monitorBounds.Y,
            monitorBounds.Width,
            monitorBounds.Height,
            0x0000
        );
    }
}