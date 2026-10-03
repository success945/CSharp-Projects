using Windows.Graphics.Capture;

static class CaptureTest
{
    public static bool IsSupported()
    {
        return GraphicsCaptureSession.IsSupported();
    }
}