using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Telnet
{
    internal class Program
    {
        const int PORT = 8080;
        static void Main(string[] args)
        {
            Socket socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint portEP = new IPEndPoint(ipAddress, PORT);
            socketListener.Bind(portEP);
            socketListener.Listen(5);
            Console.WriteLine($"Server started on port {PORT}");

            Socket client = socketListener.Accept();
            Console.WriteLine($"New client connected... {client.ToString()} IP {client.RemoteEndPoint.ToString()}");

            byte[] buffer = new byte[128];
            int numberOfRecivedBytes = 0;

            while (true)
            {
                numberOfRecivedBytes = client.Receive(buffer);
                Console.WriteLine($"Number of revived bytes: {numberOfRecivedBytes}");
                Console.WriteLine($"Data send: {buffer}");

                string resivedData = Encoding.ASCII.GetString(buffer, 0, numberOfRecivedBytes);
                Console.WriteLine($"Data from client {resivedData}");
                client.Send(buffer);


                if (resivedData == "0")
                {
                    break;
                }

                Array.Clear(buffer, 0, buffer.Length);
                numberOfRecivedBytes = 0;
            }
        }
    }
}
