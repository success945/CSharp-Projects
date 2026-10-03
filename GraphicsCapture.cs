using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Graphics.Canvas;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;

static class GraphicsCapture
{
    private static Direct3D11CaptureFramePool? framePool;
    private static GraphicsCaptureSession? session;
    private static CanvasDevice? canvasDevice;

    private static byte[]? latestPng;

    public static byte[]? GetLatestPng()
    {
        return latestPng;
    }

    public static void Start(
        IDirect3DDevice device,
        GraphicsCaptureItem item
    )
    {
        canvasDevice =
            CanvasDevice.CreateFromDirect3D11Device(
                device
            );

        framePool =
            Direct3D11CaptureFramePool.CreateFreeThreaded(
                device,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                2,
                item.Size
            );

        framePool.FrameArrived += OnFrameArrived;

        session =
            framePool.CreateCaptureSession(item);

        session.StartCapture();

        Console.WriteLine(
            $"Graphics capture started: " +
            $"{item.Size.Width} x {item.Size.Height}"
        );
    }

    private static void OnFrameArrived(
        Direct3D11CaptureFramePool sender,
        object args
    )
    {
        try
        {
            using Direct3D11CaptureFrame frame =
                sender.TryGetNextFrame();

            using CanvasBitmap canvasBitmap =
                CanvasBitmap.CreateFromDirect3D11Surface(
                    canvasDevice!,
                    frame.Surface
                );

            int width = frame.ContentSize.Width;
            int height = frame.ContentSize.Height;

            byte[] pixels =
                canvasBitmap.GetPixelBytes(
                    0,
                    0,
                    width,
                    height
                );

            using Bitmap bitmap =
                new Bitmap(
                    width,
                    height,
                    PixelFormat.Format32bppArgb
                );

            Rectangle rectangle =
                new Rectangle(
                    0,
                    0,
                    width,
                    height
                );

            BitmapData bitmapData =
                bitmap.LockBits(
                    rectangle,
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb
                );

            try
            {
                int sourceStride = width * 4;

                for (int y = 0; y < height; y++)
                {
                    IntPtr destination =
                        IntPtr.Add(
                            bitmapData.Scan0,
                            y * bitmapData.Stride
                        );

                    Marshal.Copy(
                        pixels,
                        y * sourceStride,
                        destination,
                        sourceStride
                    );
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            using MemoryStream stream =
                new MemoryStream();

            bitmap.Save(
                stream,
                ImageFormat.Png
            );

            latestPng = stream.ToArray();

            Console.WriteLine(
                $"FRAME READY: {width} x {height}"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"FRAME ERROR: {ex.Message}"
            );
        }
    }
}