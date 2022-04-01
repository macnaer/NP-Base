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
        List<EndPoint> listOfClients;

        public ChatServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            localIP = new IPEndPoint(IPAddress.Any, PORT);
            serverSocket.EnableBroadcast = true;
            listOfClients = new List<EndPoint>();
        }

        public void Run()
        {
            try
            {
                SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                socketAsyncEvent.SetBuffer(new byte[BUFFER], 0, BUFFER);
                socketAsyncEvent.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                

                if (!serverSocket.IsBound)
                {
                    serverSocket.Bind(localIP);
                }

                socketAsyncEvent.Completed += ReciveCallback;

                if (!serverSocket.ReceiveFromAsync(socketAsyncEvent))
                {
                    Console.WriteLine("Failed to recive data.");
                    Debug.WriteLine("Failed to recive data.");
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
            string command = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);
            Debug.WriteLine($"IP  {e.RemoteEndPoint}\tCommand {command}");

            if (command.Equals("<DISCOVER>"))
            {
                if (!listOfClients.Contains(e.RemoteEndPoint))
                {
                    listOfClients.Add(e.RemoteEndPoint);
                    Debug.WriteLine($"Client count: {listOfClients.Count}\nRemote address: {e.RemoteEndPoint}\n=================>");
                }
                SendCommandToClient("<CONFIRM>", e.RemoteEndPoint);
               
            }
            else
            {
                foreach (IPEndPoint remEP in listOfClients)
                {
                    if (!remEP.Equals(e.RemoteEndPoint))
                    {
                        SendCommandToClient(command, remEP);
                    }
                }
            }

            Run();
        }

        private void SendCommandToClient(string commandToSend, EndPoint remoteEndPoint)
        {
            if (string.IsNullOrEmpty(commandToSend) || remoteEndPoint == null)
            {
                return;
            }

            SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
            socketAsyncEvent.RemoteEndPoint = remoteEndPoint;

            var bytesToSend = Encoding.ASCII.GetBytes(commandToSend);

            socketAsyncEvent.SetBuffer(bytesToSend, 0, bytesToSend.Length);

            socketAsyncEvent.Completed += SendCommand;

            serverSocket.SendToAsync(socketAsyncEvent);
        }

        private void SendCommand(object? sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine($"Completed sending to client {e.RemoteEndPoint}");
            Console.WriteLine($"Completed sending to client {e.RemoteEndPoint}");
        }
    }
}
