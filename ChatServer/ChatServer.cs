using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class ChatServer
    {
        Socket serverSocket;
        IPEndPoint localIP;
        IPEndPoint localBroadcast;
        const int PORT = 8080;
        const int BUFFER = 1024;

        public ChatServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localIP = new IPEndPoint(IPAddress.Any, PORT);
            localBroadcast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), PORT);
        }

        public void Run()
        {
            try
            {
                SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                socketAsyncEvent.SetBuffer(new byte[BUFFER], 0, BUFFER);
                socketAsyncEvent.RemoteEndPoint = localBroadcast;
                socketAsyncEvent.Completed += ReciveCallback;
                serverSocket.Bind(localIP);

                if (!serverSocket.ReceiveFromAsync(socketAsyncEvent))
                {
                    Console.WriteLine("Failed to recive data.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        private void ReciveCallback(object? sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine($"Client ip {e.RemoteEndPoint}\nSize {e.Count}");
            Console.WriteLine($"Client ip {e.RemoteEndPoint}\nSize {e.Count}");
        }
    }
}
