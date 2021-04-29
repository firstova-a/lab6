using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {

        private const int serverPort = 2365;
        private static TcpClient client;
        private static string message;
        
        private static bool play = true;

        static async Task Main(string[] args)
        {
            client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, serverPort);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            Console.WriteLine("User start");
            
            while (play)
            {

                bool run = true;
                var stones = 0;
                var max_step = 0;
                var stones_s = "";
                var max_step_s = "";
                Console.Write("Enter the number of stones: ");
                stones_s = Console.ReadLine();
                Console.Write("Enter max step: ");
                max_step_s = Console.ReadLine();
                do
                {
                    if (int.TryParse(stones_s, out stones) && int.TryParse(max_step_s, out max_step) && (max_step > 0) && (stones > 0))
                    {
                        message = stones_s + ' ' + max_step_s;
                    }
                    else
                    {
                        Console.WriteLine("Input correct numbers");
                    }

                } while ((stones == 0) && (max_step == 0));

                
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
                string answer = await reader.ReadLineAsync();
                Console.WriteLine(answer);
                while (run)
                {
                    var step_s = "";
                    var step = 0;
                    do
                    {
                        Console.Write("Enter your step: ");
                        step_s = Console.ReadLine();
                        if (int.TryParse(step_s, out step) && (step > 0) && (step <= max_step))
                        {
                            Console.WriteLine("OK");
                        }
                        else
                        {
                            Console.WriteLine("Uncorrect step");
                        }
                    } while (!(int.TryParse(step_s, out step) && (step > 0) && (step <= max_step)));
                    await writer.WriteLineAsync(step_s);
                    await writer.FlushAsync();
                    answer = await reader.ReadLineAsync();
                    if ((answer == "I am winner, ha-ha!") || (answer == "You are winner!"))
                    {
                        run = false;
                    }
                    Console.WriteLine(answer);
                }
                Console.WriteLine("Want to play again? any character or no");
                if (Console.ReadLine() == "no")
                {
                    play = false;
                }


            }
            client.Close();
        }
    }
}
