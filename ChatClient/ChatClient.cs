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
        private EndPoint chatServerEP;
        byte[] BUFFER = new byte[1024];

        public ChatClient(int remotePort, int localPort)
        {
            localBroadcast = new IPEndPoint(IPAddress.Parse("255.255.255.255"), remotePort);
            localIP = new IPEndPoint(IPAddress.Any, localPort);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            clientSocket.EnableBroadcast = true;
        }

        public void Send(string str) 
        {
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            try
            {
                if (!clientSocket.IsBound)
                {
                    clientSocket.Bind(localIP);
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
                Debug.Write(e.ToString());
                throw;
            }
        }

        private void SendCallback(object? sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine($"Data send to ip {e.RemoteEndPoint}");
            if(Encoding.ASCII.GetString(e.Buffer).Equals("<CONFIRM>")){
                ReciveCommand(value: "<CONFIRM>", IP: localIP);
            }
        }

        private void ReciveCommand(string value, IPEndPoint IP)
        {
            if(IP == null)
            {
                Debug.WriteLine("Command not found");
                return;
            }

            SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
            socketAsyncEvent.SetBuffer(new byte[1024], 0, 1024);
            socketAsyncEvent.RemoteEndPoint = IP;
            socketAsyncEvent.UserToken = value;
            socketAsyncEvent.Completed += SendConfirmCallback;
            clientSocket.ReceiveFromAsync(socketAsyncEvent);
        }

        private void SendConfirmCallback(object? sender, SocketAsyncEventArgs e)
        {
            if(e.BytesTransferred == 0)
            {
                Debug.WriteLine($"0 byte transferred. {e.SocketError}");
                return;
            }

            var recivedText = Encoding.ASCII.GetString(e.Buffer, 0, e.BytesTransferred);
            var expectedText = Convert.ToString(e.UserToken);

            if (recivedText.Equals(expectedText))
            {
                Debug.WriteLine($"Recived confirmation from server {e.RemoteEndPoint}");
                chatServerEP = e.RemoteEndPoint;
            }
            else if (string.IsNullOrEmpty(expectedText) &&
               !string.IsNullOrEmpty(recivedText))
            {
                Debug.WriteLine($"Text received: {recivedText}");
                ReciveCommand(string.Empty, chatServerEP as IPEndPoint);
            }
            else if (!string.IsNullOrEmpty(expectedText) && !recivedText.Equals(expectedText))
            {
                Debug.WriteLine($"Expected token not returned by the server.");                 
            }
        }

        public void SendMessageToKnownServer(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message))
                {
                    return;
                }

                var bytesToSend = Encoding.ASCII.GetBytes(message);

                SocketAsyncEventArgs socketAsyncEvent = new SocketAsyncEventArgs();
                socketAsyncEvent.SetBuffer(bytesToSend, 0, bytesToSend.Length);

                socketAsyncEvent.RemoteEndPoint = chatServerEP;

                socketAsyncEvent.UserToken = message;

                socketAsyncEvent.Completed += SendMessageToKnownServerCompletedCallback;

                clientSocket.SendToAsync(socketAsyncEvent);

            }
            catch (Exception excp)
            {

                Debug.WriteLine(excp.ToString());
            }
        }

        private void SendMessageToKnownServerCompletedCallback(object? sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine($"Sent: {e.UserToken}{Environment.NewLine}Server: {e.RemoteEndPoint}");
        }
    }
}
