using System;
using System.Runtime.InteropServices;
using Windows.Graphics.Capture;
using WinRT;

static class CaptureItemHelper
{
    private static readonly Guid GraphicsCaptureItemGuid =
        new Guid("79C3F95B-31F7-4EC2-A464-632EF5D30760");

    [ComImport]
    [Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    private interface IGraphicsCaptureItemInterop
    {
        IntPtr CreateForWindow(
            [In] IntPtr window,
            [In] ref Guid iid
        );

        IntPtr CreateForMonitor(
            [In] IntPtr monitor,
            [In] ref Guid iid
        );
    }

    public static GraphicsCaptureItem CreateForWindow(
        IntPtr windowHandle
    )
    {
        var interop =
            GraphicsCaptureItem
                .As<IGraphicsCaptureItemInterop>();

        Guid iid = GraphicsCaptureItemGuid;

        IntPtr itemPointer =
            interop.CreateForWindow(
                windowHandle,
                ref iid
            );

        try
        {
            return MarshalInterface<GraphicsCaptureItem>
                .FromAbi(itemPointer);
        }
        finally
        {
            Marshal.Release(itemPointer);
        }
    }
}