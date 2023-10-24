using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpClient
{
    class Program
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int Port = 100;

        static void Main()
        {
            Console.Title = "Client";
            ConnectToServer();
            RequestLoop();

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
        }

        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    ClientSocket.Connect(IPAddress.Loopback, Port);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }
            Console.Clear();
            Console.WriteLine("Connected");
        }

        private static void RequestLoop()
        {
            Console.WriteLine(@"<Type ""exit"" to properly disconnect client>");
            Console.WriteLine(@"<Type ""play"" to start to play three in a row>");
            string requestSent = string.Empty;


              try
               {
                while (requestSent.ToLower() != "exit" || requestSent.ToLower() != "q")
                {
                    char[] gamePlan;
                    Console.Write("Send a request: ");
                    requestSent = Console.ReadLine();
                    ClientSocket.Send(Encoding.UTF8.GetBytes(requestSent), SocketFlags.None);
                    gamePlan = ReceiveResponse();

                        while (gamePlan[0] == 'P')
                        {
                            requestSent = "playon " + string.Join("", gamePlan);
                            ClientSocket.Send(Encoding.UTF8.GetBytes(requestSent), SocketFlags.None);
                            gamePlan = ReceiveResponse();
                        }        
                }


              }
              catch (Exception)
              {
                ClientSocket.Send(Encoding.UTF8.GetBytes("exit"), SocketFlags.None);
              
                Console.WriteLine("Error! - Lost server.");
              }

        }

        private static char[] ReceiveResponse()
        {
            var buffer = new byte[2048];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) 
                return null;

            char[] gamePlan = Encoding.UTF8.GetString(buffer, 0, received).ToCharArray();
          
                GamePlan(gamePlan);
               
            return gamePlan;
        }

        private static void GamePlan(char[] gamePlan)
        {
            Console.Clear();
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", gamePlan[1], gamePlan[2], gamePlan[3]);
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", gamePlan[4], gamePlan[5], gamePlan[6]);
            Console.WriteLine("_____|_____|_____ ");
            Console.WriteLine("     |     |      ");
            Console.WriteLine("  {0}  |  {1}  |  {2}", gamePlan[7], gamePlan[8], gamePlan[9]);
            Console.WriteLine("     |     |      ");

                if (gamePlan[0] != 'D')
                {
                if (gamePlan[0] == 'X') Console.WriteLine("you win");
                else if (gamePlan[0] == 'O') Console.WriteLine("Computer wins");
                else gamePlan = ChooseSlot(gamePlan);
                }
                else Console.WriteLine("Game over....");

            
        }

        private static char[] ChooseSlot(char[] gamePlan)
        {
            Console.WriteLine("Choose number where to place your slot(X)");

            try
            {
                int MyNum = Convert.ToInt32(Console.ReadLine());
                gamePlan[MyNum] = 'X';
            }
            catch (Exception)
            { 
                Console.WriteLine("Error! - Wrong number.");
            }

            
           
            return gamePlan;
        }

    }
}
