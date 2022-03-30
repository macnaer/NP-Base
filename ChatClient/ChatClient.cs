using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
   
    public class ChatClient
    {
        Socket clientSocket;
        IPEndPoint localIP;
        IPEndPoint localBroadcast;
        byte[] BUFFER = new byte[1024];

        public ChatClient(int remotePort, int localPort)
        {
            localBroadcast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), remotePort);
            localIP = new IPEndPoint(IPAddress.Any, localPort);
        }

        public void Send(string str) 
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }

                if(clientSocket == null)
                {
                    clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    clientSocket.EnableBroadcast = true;
                    clientSocket.Bind(localIP);
                }

                SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                BUFFER = Encoding.ASCII.GetBytes(str);
                socketAsyncEvent.SetBuffer(BUFFER, 0 , BUFFER.Length);
                socketAsyncEvent.RemoteEndPoint = localBroadcast;
                socketAsyncEvent.Completed += SendCallback;
                clientSocket.SendToAsync(socketAsyncEvent);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                throw;
            }
        }

        private void SendCallback(object? sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine($"Server ip {e.RemoteEndPoint}\nSize {e.Count}");
            Console.WriteLine($"Server ip {e.RemoteEndPoint}\nSize {e.Count}");
        }
    }
}
