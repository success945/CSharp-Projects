using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

static class MonitorFinder
{
    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(
        POINT pt,
        uint dwFlags
    );

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    public static IntPtr ChooseMonitor()
    {
        Screen[] screens = Screen.AllScreens;

        Console.WriteLine();
        Console.WriteLine("Choose a display to cast:");
        Console.WriteLine();

        for (int i = 0; i < screens.Length; i++)
        {
            Screen screen = screens[i];

            Console.WriteLine(
                $"{i + 1}. {screen.DeviceName} " +
                $"({screen.Bounds.Width} x {screen.Bounds.Height})" +
                (screen.Primary ? " [PRIMARY]" : "")
            );
        }

        Console.WriteLine();
        Console.Write("Enter number: ");

        int choice = int.Parse(Console.ReadLine()!);

        Screen selected = screens[choice - 1];

        POINT point = new POINT
        {
            X = selected.Bounds.Left + 1,
            Y = selected.Bounds.Top + 1
        };

        return MonitorFromPoint(point, 2);
    }
}