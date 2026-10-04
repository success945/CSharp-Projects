using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

//ApplicationConfiguration.Initialize();
//Application.Run(new MainForm());
//return;

#if false
IntPtr selectedWindow =
    WindowFinder.ChooseWindow();
Console.WriteLine(
    $"Selected window handle: {selectedWindow}"
);

var captureItem =
    CaptureItemHelper.CreateForWindow(
        selectedWindow
    );
#endif
IntPtr selectedMonitor = MonitorFinder.ChooseMonitor();

Console.WriteLine($"Selected monitor handle: {selectedMonitor}");

var captureItem =
    CaptureItemHelper.CreateForMonitor(selectedMonitor);

Console.WriteLine(
    $"Windows Graphics Capture item created: {captureItem.DisplayName}"
);

var graphicsDevice =
    Direct3DHelper.CreateDevice();

Console.WriteLine(
    "Direct3D capture device created successfully."
);

GraphicsCapture.Start(
    graphicsDevice,
    captureItem
);

Thread previewThread = new Thread(() =>
{
    ApplicationConfiguration.Initialize();
    Application.Run(new PreviewForm());
});

previewThread.SetApartmentState(ApartmentState.STA);
previewThread.IsBackground = true;
previewThread.Start();

#if false
WindowFinder.GetWindowRect(
    selectedWindow,
    out WindowFinder.RECT windowRect
);

int windowWidth =
    windowRect.Right - windowRect.Left;

int windowHeight =
    windowRect.Bottom - windowRect.Top;

/*Rectangle screenSize = new Rectangle(
    0,
    0,
    683,
    384
);*/


Rectangle screenSize = new Rectangle(
    windowRect.Left,
    windowRect.Top,
    windowWidth,
    windowHeight
);
#endif

HttpListener server = new HttpListener();

server.Prefixes.Add("http://*:8080/");

server.Start();


#if false
Console.WriteLine(
    $"Window position: {windowRect.Left}, {windowRect.Top}"
);
Console.WriteLine(
    $"Window size: {windowWidth} x {windowHeight}"
);
#endif

Console.WriteLine("SimpleCast server is running.");

while (true)
{
Console.WriteLine("Waiting for a connection...");

HttpListenerContext connection = server.GetContext();
string path = connection.Request.Url.AbsolutePath;

#if false
/*if (path == "/screen")
{*/
/*string message = "Hello from SimpleCast!";
byte[] data = System.Text.Encoding.UTF8.GetBytes(message);*/

using Bitmap screenshot = new Bitmap(
    screenSize.Width,
    screenSize.Height
);

using Graphics graphics = Graphics.FromImage(screenshot);

graphics.CopyFromScreen(
    screenSize.X,
    screenSize.Y,
    0,
    0,
    screenSize.Size
); */

/*using Bitmap screenshot =
    WindowCapture.Capture(selectedWindow);

Console.WriteLine(
    "Screenshot captured!"
);*/

/*screenshot.Save(
    "screenshot.png",
    ImageFormat.Png
);

byte[] data = File.ReadAllBytes("screenshot.png");*/
/*connection.Response.ContentType = "image/png";*/
/*connection.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
connection.Response.Headers.Add("Pragma", "no-cache");
connection.Response.Headers.Add("Expires", "0");

using MemoryStream memory = new MemoryStream();

screenshot.Save(
    memory,
    ImageFormat.Png
);
byte[] data = memory.ToArray();

connection.Response.ContentLength64 = data.Length;
try {
connection.Response.OutputStream.Write(data, 0, data.Length);
connection.Response.OutputStream.Close();

Console.WriteLine("Message sent!");
} // close try
catch (System.Net.HttpListenerException)
{
    Console.WriteLine("Connection closed by device");
} // close catch
} */ // close if 
#endif
if (path == "/screen")
{
    byte[]? data =
        GraphicsCapture.GetLatestPng();

    if (data == null)
    {
        string message =
            "Waiting for first captured frame...";

        byte[] waitingData =
            System.Text.Encoding.UTF8.GetBytes(message);

        connection.Response.ContentType =
            "text/plain";

        connection.Response.ContentLength64 =
            waitingData.Length;

        connection.Response.OutputStream.Write(
            waitingData,
            0,
            waitingData.Length
        );

        connection.Response.OutputStream.Close();

        continue;
    }

    connection.Response.ContentType =
        "image/png";

    connection.Response.ContentLength64 =
        data.Length;

    try
    {
        connection.Response.OutputStream.Write(
            data,
            0,
            data.Length
        );

        connection.Response.OutputStream.Close();
    }
    catch (HttpListenerException)
    {
        Console.WriteLine(
            "Connection closed by device"
        );
    }
}
else
{
    // string page = "SimpleCast Receiver";
    // string page = "<html><body><img src='/screen' style='width:100%;'><body><html>";
/*string page = @"
<html>
<body>
    <img id='screen' src='/screen' style='width:100%;'>

    <script>
        setInterval(function() {
            document.getElementById('screen').src =
                '/screen?t=' + Date.now();
        }, 1000);
    </script>
</body>
</html>";*/

string page = @"
<html>
<head>
    <style>
        html, body {
            margin: 0;
            width: 100%;
            height: 100%;
            background: black;
            overflow: hidden;
        }

        #screen {
            width: 100%;
            height: 100%;
            object-fit: contain;
        }
    </style>
</head>

<body>
    <img id='screen' src='/screen'>

    <script>
        setInterval(function() {
            document.getElementById('screen').src =
                '/screen?t=' + Date.now();
        }, 500);
    </script>
</body>
</html>";

    byte[] pageData =
        System.Text.Encoding.UTF8.GetBytes(page);

    connection.Response.ContentType = "text/html";
    connection.Response.ContentLength64 = pageData.Length;

    connection.Response.OutputStream.Write(
        pageData,
        0,
        pageData.Length
    );

    connection.Response.OutputStream.Close();
/*
string page = @"
<html>
<body style='background:white;'>
    <h1 style='color:red;font-size:80px;'>
        SIMPLECAST TEST
    </h1>
</body>
</html>"; */
}
} // close while