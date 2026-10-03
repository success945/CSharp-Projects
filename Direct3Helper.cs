using System;
using System.Runtime.InteropServices;
using Windows.Graphics.DirectX.Direct3D11;

static class Direct3DHelper
{
    private const uint D3D11_CREATE_DEVICE_BGRA_SUPPORT = 0x20;

    [DllImport("d3d11.dll")]
    private static extern int D3D11CreateDevice(
        IntPtr pAdapter,
        int driverType,
        IntPtr software,
        uint flags,
        IntPtr featureLevels,
        uint featureLevelsCount,
        uint sdkVersion,
        out IntPtr device,
        out int featureLevel,
        out IntPtr immediateContext
    );

    [DllImport("d3d11.dll")]
    private static extern int CreateDirect3D11DeviceFromDXGIDevice(
        IntPtr dxgiDevice,
        out IntPtr graphicsDevice
    );

    public static IDirect3DDevice CreateDevice()
    {
        const int D3D_DRIVER_TYPE_HARDWARE = 1;
        const uint D3D11_SDK_VERSION = 7;

        int result = D3D11CreateDevice(
            IntPtr.Zero,
            D3D_DRIVER_TYPE_HARDWARE,
            IntPtr.Zero,
            D3D11_CREATE_DEVICE_BGRA_SUPPORT,
            IntPtr.Zero,
            0,
            D3D11_SDK_VERSION,
            out IntPtr d3dDevice,
            out _,
            out IntPtr context
        );

        if (result < 0)
        {
            Marshal.ThrowExceptionForHR(result);
        }

        Marshal.Release(context);

        Guid dxgiDeviceGuid =
            new Guid("54EC77FA-1377-44E6-8C32-88FD5F44C84C");

        result = Marshal.QueryInterface(
            d3dDevice,
            ref dxgiDeviceGuid,
            out IntPtr dxgiDevice
        );

        Marshal.Release(d3dDevice);

        if (result < 0)
        {
            Marshal.ThrowExceptionForHR(result);
        }

        result = CreateDirect3D11DeviceFromDXGIDevice(
            dxgiDevice,
            out IntPtr graphicsDevice
        );

        Marshal.Release(dxgiDevice);

        if (result < 0)
        {
            Marshal.ThrowExceptionForHR(result);
        }

        try
        {
            return WinRT.MarshalInterface<IDirect3DDevice>
                .FromAbi(graphicsDevice);
        }
        finally
        {
            Marshal.Release(graphicsDevice);
        }
    }
}