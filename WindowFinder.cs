using System;
using System.Collections.Generic;
using System.Text;

static class WindowFinder
{
    // This describes the function Windows will use
    // while going through all open windows.
    public delegate bool EnumWindowsProc(
        IntPtr hWnd,
        IntPtr lParam
    );


    // Ask Windows for all currently open windows.
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool EnumWindows(
        EnumWindowsProc enumProc,
        IntPtr lParam
    );


    // Get the title/name of a window.
    [System.Runtime.InteropServices.DllImport(
        "user32.dll",
        CharSet = System.Runtime.InteropServices.CharSet.Unicode
    )]
    public static extern int GetWindowText(
        IntPtr hWnd,
        StringBuilder text,
        int count
    );


    // Check whether the window is actually visible.
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool IsWindowVisible(
        IntPtr hWnd
    );


    // Find all visible windows and let the user choose one.
    public static IntPtr ChooseWindow()
    {
        List<IntPtr> handles = new List<IntPtr>();
        List<string> titles = new List<string>();


        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder title =
                    new StringBuilder(256);

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
        },
        IntPtr.Zero);


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
            GetWindowText(hWnd, title, title.Capacity);

            if (title.Length > 0)
            {
                windows.Add(new WindowInfo
                {
                    Handle = hWnd,
                    Title = title.ToString()
                });
            }
        }

        return true;
    }, IntPtr.Zero);

    return windows;
}
}