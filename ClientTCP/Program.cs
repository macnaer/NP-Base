using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientTCP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipaddress = null;
            int port = 0;

            try
            {
                Console.WriteLine("==== ITSTEP CLIENT ===");

                Console.WriteLine("Enter ip address:");
                string userIPAddress = Console.ReadLine();

                Console.WriteLine("Enter port 0 - 65535:");
                string userPort = Console.ReadLine(); 

                if(userIPAddress == "")
                {
                    userIPAddress = "127.0.0.1";
                }

                if(userPort == "")
                {
                    userPort = "8080";
                }

                if (!IPAddress.TryParse(userIPAddress, out ipaddress))
                {
                    Console.WriteLine("Invalid ip address.");
                    return;
                }

                if(!int.TryParse(userPort.Trim(), out port))
                {
                    Console.WriteLine("Invalid port.");
                    return;
                }

                if (port <= 0 || port > 65535)
                {
                    Console.WriteLine("Port must be between 0 and 65535");
                    return;
                }

                Console.WriteLine($"IP Address {ipaddress.ToString()} - Port {port}");
                client.Connect(ipaddress, port);

                Console.WriteLine("Press <Exit> for close connection.");

                string command = String.Empty;

                while (true)
                {
                    command = Console.ReadLine();
                    if (command.Equals("<Exit>"))
                    {
                        break;
                    }

                    byte[] bufferSend = Encoding.ASCII.GetBytes(command);
                    client.Send(bufferSend);

                    byte[] bufferRecived = new byte[128];
                    int recive = client.Receive(bufferRecived);
                    string resivedData = Encoding.ASCII.GetString(bufferRecived, 0, recive);
                    Console.WriteLine($"Data recived {resivedData}");
                }


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if(client != null)
                {
                    if (client.Connected)
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }
                    client.Close();
                    client.Dispose();
                }
            }
        }
    }
}
