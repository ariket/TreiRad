using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpServer
{
    class Program
    {
        private static Socket _serverSocket;
        private static readonly List<Socket> ClientSockets = new List<Socket>();
        private const int BufferSize = 2048;
        private const int Port = 100;
        private static readonly byte[] Buffer = new byte[BufferSize];
        private static bool _closing;

        static void Main()
        {
            Console.Title = "Server";
            SetupServer();

            //Vänta här!
            Console.ReadLine(); 
            _closing = true;
            CloseAllSockets();
            Thread.Sleep(2000);
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _serverSocket.Listen(5);
            _serverSocket.BeginAccept(AcceptCallback, null);
            Console.WriteLine("Server setup complete");
        }

        private static void CloseAllSockets()
        {
            foreach (Socket socket in ClientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }

            _serverSocket.Close();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            if (_closing)
                return;

            Socket socket = _serverSocket.EndAccept(ar);
            ClientSockets.Add(socket);
            socket.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, socket);
            Console.WriteLine("Client connected, waiting for request...");
            _serverSocket.BeginAccept(AcceptCallback, null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            if (_closing)
                return;

            Socket current = (Socket)ar.AsyncState;
            int received;

            try
            {
                received = current.EndReceive(ar);
            }
            catch (SocketException)
            {
                Console.WriteLine("Client forcefully disconnected");
                current.Close(); 
                ClientSockets.Remove(current);
                return;
            }

            string text = Encoding.UTF8.GetString(Buffer, 0, received);
            Console.WriteLine("Received Text: " + text);

            switch (text.ToLower())
            {
                case "get time":
                    Console.WriteLine("Text is a get time request");
                    current.Send(Encoding.UTF8.GetBytes(DateTime.Now.ToLongTimeString()));
                    Console.WriteLine("Time sent to client");
                    break;
                case "exit":
                    current.Shutdown(SocketShutdown.Both);
                    current.Close();
                    ClientSockets.Remove(current);
                    Console.WriteLine("Client disconnected");
                    return;
                    break;
                default:
                    Console.WriteLine("Text is an invalid request");
                    current.Send(Encoding.UTF8.GetBytes("Invalid request"));
                    Console.WriteLine("Warning Sent");
                    break;
            }

            current.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
        }
    }
}
