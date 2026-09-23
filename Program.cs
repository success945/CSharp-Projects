using System.Net;

HttpListener server = new HttpListener();

server.Prefixes.Add("http://*:8080/");

server.Start();

Console.WriteLine("SimpleCast server is running.");
Console.WriteLine("Waiting for a connection...");

HttpListenerContext connection = server.GetContext();

string message = "Hello from SimpleCast!";

byte[] data = System.Text.Encoding.UTF8.GetBytes(message);

connection.Response.ContentLength64 = data.Length;
connection.Response.OutputStream.Write(data, 0, data.Length);
connection.Response.OutputStream.Close();

Console.WriteLine("Message sent!");