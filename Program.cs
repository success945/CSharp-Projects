Console.WriteLine("Enter receiver IP address: ");
string address = Console.ReadLine();
Console.WriteLine("You entered: " + address);
if (address == "192.168.1.20")
{
    Console.WriteLine("Receiver found!");
}
else
{Console.WriteLine("Receiver not found");}
