using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace TcpServerSingle
{
    class Program
    {
        private static Socket _serverSocket;
        private const int BufferSize = 2048;
        private const int Port = 100;
        private static readonly byte[] Buffer = new byte[BufferSize];
        private static bool _closing;

        static void Main()
        {
            Console.Title = "Server";

            Console.WriteLine("Setting up server...");
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _serverSocket.Listen(5);
            Console.WriteLine("Waiting for connection...");
            var clientSocket = _serverSocket.Accept();
            
            Console.WriteLine("Client connected, waiting for request...");

            clientSocket.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, clientSocket);
            // Vänta här!
            Console.ReadLine(); 
            _closing = true;

            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            _serverSocket.Close();
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
                return;
            }

            string text = Encoding.UTF8.GetString(Buffer, 0, received);
            Console.WriteLine("Received Text: " + text);
            string[] argument = text.Split();
           

            switch (argument[0])
            {
                case "play":
                    char[] gamePlan = new char[] {'P', '1', '2', '3', '4', '5', '6', '7', '8', '9'}; //"number" = empty slot, client = "X", server = "O"
                    Console.WriteLine("Client wants to play");
                    current.Send(Encoding.UTF8.GetBytes(string.Join("",gamePlan)));
                    Console.WriteLine("gameplan sent to client");
                    break;
                case "playon":
                    Console.WriteLine("Play ongoing");
                    gamePlan = PlayThreeInARow(argument[1].ToCharArray());
                    current.Send(Encoding.UTF8.GetBytes(string.Join("", gamePlan)));
                    Console.Write("gameplan sent to client  ");
                    Console.WriteLine(gamePlan[0]);
                    break;
                case "exit": case "q": case "quit":
                    current.Shutdown(SocketShutdown.Both);
                    current.Close();
                    Console.WriteLine("Client disconnected");
                    return;
                default:
                    Console.WriteLine("Text is an invalid request");
                    current.Send(Encoding.UTF8.GetBytes("Invalid request"));
                    Console.WriteLine("Warning Sent");
                    break;
            }

            current.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, current);
        }

        private static char[] PlayThreeInARow(char[] gameplan)
        {
            char res = CheckWin(gameplan);

            if (res != 'X' && res != 'D')
            {
                if (gameplan[5] == '5')
                {
                    gameplan[5] = 'O';
                }

                else if (gameplan[1] == 'O' && gameplan[2] == 'O' && gameplan[3] == '3')
                {
                    gameplan[3] = 'O';
                }

                else if (gameplan[1] == '1' && gameplan[2] == 'O' && gameplan[3] == 'O')
                {
                    gameplan[1] = 'O';
                }

                else if (gameplan[1] == 'O' && gameplan[2] == '2' && gameplan[3] == 'O')
                {
                    gameplan[2] = 'O';
                }
                else if (gameplan[4] == 'O' && gameplan[5] == 'O' && gameplan[6] == '6')
                {
                    gameplan[6] = 'O';
                }

                else if (gameplan[4] == '4' && gameplan[5] == 'O' && gameplan[6] == 'O')
                {
                    gameplan[4] = 'O';
                }

                else if (gameplan[4] == 'O' && gameplan[5] == '5' && gameplan[6] == 'O')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[7] == 'O' && gameplan[8] == 'O' && gameplan[9] == '9')
                {
                    gameplan[9] = 'O';
                }

                else if (gameplan[7] == '7' && gameplan[8] == 'O' && gameplan[9] == 'O')
                {
                    gameplan[7] = 'O';
                }

                else if (gameplan[7] == 'O' && gameplan[8] == '8' && gameplan[9] == 'O')
                {
                    gameplan[8] = 'O';
                }
                else if (gameplan[1] == 'O' && gameplan[4] == 'O' && gameplan[7] == '7')
                {
                    gameplan[9] = 'O';
                }
                else if (gameplan[1] == 'O' && gameplan[4] == '4' && gameplan[7] == 'O')
                {
                    gameplan[4] = 'O';
                }
                else if (gameplan[1] == '1' && gameplan[4] == 'O' && gameplan[7] == 'O')
                {
                    gameplan[1] = 'O';
                }
                else if (gameplan[2] == 'O' && gameplan[5] == 'O' && gameplan[8] == '8')
                {
                    gameplan[8] = 'O';
                }
                else if (gameplan[2] == 'O' && gameplan[5] == '5' && gameplan[8] == 'O')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[2] == '2' && gameplan[5] == 'O' && gameplan[8] == 'O')
                {
                    gameplan[2] = 'O';
                }
                else if (gameplan[3] == 'O' && gameplan[6] == 'O' && gameplan[9] == '9')
                {
                    gameplan[9] = 'O';
                }
                else if (gameplan[3] == 'O' && gameplan[6] == '6' && gameplan[9] == 'O')
                {
                    gameplan[6] = 'O';
                }
                else if (gameplan[3] == '3' && gameplan[6] == 'O' && gameplan[9] == 'O')
                {
                    gameplan[3] = 'O';
                }
                else if (gameplan[1] == 'O' && gameplan[5] == 'O' && gameplan[9] == '9')
                {
                    gameplan[9] = 'O';
                }
                else if (gameplan[1] == 'O' && gameplan[5] == '5' && gameplan[9] == 'O')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[1] == '1' && gameplan[5] == 'O' && gameplan[9] == 'O')
                {
                    gameplan[1] = 'O';
                }
                else if (gameplan[3] == 'O' && gameplan[5] == 'O' && gameplan[7] == '7')
                {
                    gameplan[7] = 'O';
                }
                else if (gameplan[3] == 'O' && gameplan[5] == '5' && gameplan[7] == 'O')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[3] == '3' && gameplan[5] == 'O' && gameplan[7] == 'O')
                {
                    gameplan[3] = 'O';
                }
                //Defense follows
                else if (gameplan[1] == 'X' && gameplan[2] == 'X' && gameplan[3] == '3')
                {
                    gameplan[3] = 'O';
                }

                else if (gameplan[1] == '1' && gameplan[2] == 'X' && gameplan[3] == 'X')
                {
                    gameplan[1] = 'O';
                }

                else if (gameplan[1] == 'X' && gameplan[2] == '2' && gameplan[3] == 'X')
                {
                    gameplan[2] = 'O';
                }
                else if (gameplan[4] == 'X' && gameplan[5] == 'X' && gameplan[6] == '6')
                {
                    gameplan[6] = 'O';
                }

                else if (gameplan[4] == '4' && gameplan[5] == 'X' && gameplan[6] == 'X')
                {
                    gameplan[4] = 'O';
                }

                else if (gameplan[4] == 'X' && gameplan[5] == '5' && gameplan[6] == 'X')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[7] == 'X' && gameplan[8] == 'X' && gameplan[9] == '9')
                {
                    gameplan[9] = 'O';
                }

                else if (gameplan[7] == '7' && gameplan[8] == 'X' && gameplan[9] == 'X')
                {
                    gameplan[7] = 'O';
                }

                else if (gameplan[7] == 'X' && gameplan[8] == '8' && gameplan[9] == 'X')
                {
                    gameplan[8] = 'O';
                }
                else if (gameplan[1] == 'X' && gameplan[4] == 'X' && gameplan[7] == '7')
                {
                    gameplan[7] = 'O';
                }
                else if (gameplan[1] == 'X' && gameplan[4] == '4' && gameplan[7] == 'X')
                {
                    gameplan[4] = 'O';
                }
                else if (gameplan[1] == '1' && gameplan[4] == 'X' && gameplan[7] == 'X')
                {
                    gameplan[1] = 'O';
                }
                else if (gameplan[2] == 'X' && gameplan[5] == 'X' && gameplan[8] == '8')
                {
                    gameplan[8] = 'O';
                }
                else if (gameplan[2] == 'X' && gameplan[5] == '5' && gameplan[8] == 'X')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[2] == '2' && gameplan[5] == 'X' && gameplan[8] == 'X')
                {
                    gameplan[2] = 'O';
                }
                else if (gameplan[3] == 'X' && gameplan[6] == 'X' && gameplan[9] == '9')
                {
                    gameplan[9] = 'O';
                }
                else if (gameplan[3] == 'X' && gameplan[6] == '6' && gameplan[9] == 'X')
                {
                    gameplan[6] = 'O';
                }
                else if (gameplan[3] == '3' && gameplan[6] == 'X' && gameplan[9] == 'X')
                {
                    gameplan[3] = 'O';
                }
                else if (gameplan[1] == 'X' && gameplan[5] == 'X' && gameplan[9] == '9')
                {
                    gameplan[9] = 'O';
                }
                else if (gameplan[1] == 'X' && gameplan[5] == '5' && gameplan[9] == 'X')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[1] == '1' && gameplan[5] == 'X' && gameplan[9] == 'X')
                {
                    gameplan[1] = 'O';
                }
                else if (gameplan[3] == 'X' && gameplan[5] == 'X' && gameplan[7] == '7')
                {
                    gameplan[7] = 'O';
                }
                else if (gameplan[3] == 'X' && gameplan[5] == '5' && gameplan[7] == 'X')
                {
                    gameplan[5] = 'O';
                }
                else if (gameplan[3] == '3' && gameplan[5] == 'X' && gameplan[7] == 'X')
                {
                    gameplan[3] = 'O';
                }
                else
                {
                    Random rnd = new Random();
                    int number = rnd.Next(1, 10);

                    while (gameplan[number] == 'O' || gameplan[number] == 'X')
                    {
                        number = rnd.Next(1, 10);
                    }
                    gameplan[number] = 'O';
                }
                res = CheckWin(gameplan);
            }
            
            gameplan[0] = res;

            return gameplan;
        }

        private static char[] AIComputer(char[] gameplan)
        {
            if (gameplan[5] == '5')
            {
                gameplan[5] = 'O';
            }

            else if (gameplan[1] == 'O' && gameplan[2] == 'O' && gameplan[3] == '3')
            {
                gameplan[3] = 'O';
            }


            return gameplan;
        }


        //Checking if any player has won or not
        private static char CheckWin(char[] gameplan)
        {
           
            if (gameplan[1] == gameplan[2] && gameplan[2] == gameplan[3])
            {
                if (gameplan[1] == 'O') return 'O';
                else return 'X';
            }
            
            else if (gameplan[4] == gameplan[5] && gameplan[5] == gameplan[6])
            {
                if(gameplan[4] == 'O') return 'O';
                else return 'X';
            }
            
            else if (gameplan[7] == gameplan[8] && gameplan[8] == gameplan[9])
            {
                if (gameplan[7] == 'O') return 'O';
                else return 'X';
            }
           
            else if (gameplan[1] == gameplan[4] && gameplan[4] == gameplan[7])
            {
                if (gameplan[1] == 'O') return 'O';
                else return 'X';
            }
            
            else if (gameplan[2] == gameplan[5] && gameplan[5] == gameplan[8])
            {
                if (gameplan[2] == 'O') return 'O';
                else return 'X';
            }
            
            else if (gameplan[3] == gameplan[6] && gameplan[6] == gameplan[9])
            {
                if (gameplan[3] == 'O') return 'O';
                else return 'X';
            }
            
            else if (gameplan[1] == gameplan[5] && gameplan[5] == gameplan[9])
            {
                if (gameplan[1] == 'O') return 'O';
                else return 'X';
            }
            else if (gameplan[3] == gameplan[5] && gameplan[5] == gameplan[7])
            {
                if (gameplan[3] == 'O') return 'O';
                else return 'X';
            }
           
            // If all the cells or values filled with X or O then no player has won the match(Draw)
            else if (gameplan[1] != '1' && gameplan[2] != '2' && gameplan[3] != '3' && gameplan[4] != '4' && gameplan[5] != '5' && gameplan[6] != '6' && gameplan[7] != '7' && gameplan[8] != '8' && gameplan[9] != '9')
            {
                return 'D';
            }
           
            else
            {
                return 'P';
            }
        }

    }
}
