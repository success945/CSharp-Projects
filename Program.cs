using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

Rectangle screenSize = new Rectangle(
    0,
    0,
    1366,
    768
);

Bitmap screenshot = new Bitmap(
    screenSize.Width,
    screenSize.Height
);

Graphics graphics = Graphics.FromImage(screenshot);

graphics.CopyFromScreen(
    screenSize.X,
    screenSize.Y,
    0,
    0,
    screenSize.Size
);

screenshot.Save(
    "screenshot.png",
    ImageFormat.Png
);

Console.WriteLine(
    "Screenshot captured!"
);

HttpListener server = new HttpListener();

server.Prefixes.Add("http://*:8080/");

server.Start();

Console.WriteLine("SimpleCast server is running.");
Console.WriteLine("Waiting for a connection...");

HttpListenerContext connection = server.GetContext();

/*string message = "Hello from SimpleCast!";
byte[] data = System.Text.Encoding.UTF8.GetBytes(message);*/

byte[] data = File.ReadAllBytes("screenshot.png");
connection.Response.ContentType = "image/png";

connection.Response.ContentLength64 = data.Length;
connection.Response.OutputStream.Write(data, 0, data.Length);
connection.Response.OutputStream.Close();

Console.WriteLine("Message sent!");