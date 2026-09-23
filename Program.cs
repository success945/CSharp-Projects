using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

Rectangle screenSize = new Rectangle(
    0,
    0,
    683,
    384
);




HttpListener server = new HttpListener();

server.Prefixes.Add("http://*:8080/");

server.Start();

Console.WriteLine("SimpleCast server is running.");

while (true)
{
Console.WriteLine("Waiting for a connection...");

HttpListenerContext connection = server.GetContext();
string path = connection.Request.Url.AbsolutePath;
if (path == "/screen")
{
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
);

Console.WriteLine(
    "Screenshot captured!"
);

/*screenshot.Save(
    "screenshot.png",
    ImageFormat.Png
);

byte[] data = File.ReadAllBytes("screenshot.png");*/
connection.Response.ContentType = "image/png";

using MemoryStream memory = new MemoryStream();

screenshot.Save(
    memory,
    ImageFormat.Png
);
byte[] data = memory.ToArray();

connection.Response.ContentLength64 = data.Length;
connection.Response.OutputStream.Write(data, 0, data.Length);
connection.Response.OutputStream.Close();

Console.WriteLine("Message sent!");
} // close if
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
}
} // close while