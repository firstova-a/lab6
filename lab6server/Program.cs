using System;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace Server
{
    class Program
    {
        private static TcpListener listener;
        private const int serverPort = 2365;
        private static bool run;
        private static int max_iter;
        private static int stones;

        private static TcpClient client;
        private static NetworkStream stream;
        private static StreamReader reader;
        private static StreamWriter writer;



        static async Task Main(string[] args)
        {
            Console.WriteLine("Server start");
            listener = new TcpListener(IPAddress.Any, serverPort);
            run = true;
            while (true)
            {
                await Listen();
            }
            }

        private static async Task Listen()
        {
            listener.Start();
            client = await listener.AcceptTcpClientAsync();
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);


            while (run)
            {


                bool game_data = false;
                while (game_data == false)
                {
                    string message = await reader.ReadLineAsync();
                    if (message != null)
                    {
                        string[] listMessage = message.Split(" ");
                        if (int.TryParse(listMessage[0], out stones) && int.TryParse(listMessage[1], out max_iter) && (stones > 0) && (max_iter > 0))
                        {
                            game_data = true;
                        }
                    }
                }
               
                    await Game();
                
                


            }



        }

        private static async Task Game()
        {
            int client_step;
            int serv_step;
            string to_client;
            Random random = new Random();
            bool game = true;
            while (game)
            {
                if (stones > 0)
                {
                    if (stones % (max_iter + 1) == 0)
                    {
                        serv_step = random.Next(1, max_iter);
                    }
                    else
                    {
                        serv_step = stones % (max_iter + 1);
                    }

                    stones -= serv_step;
                    if (stones < 1)
                    {
                        writer = new StreamWriter(stream);
                        await writer.WriteLineAsync("I am winner, ha-ha!");
                        await writer.FlushAsync();
                        game = false;
                        break;
                    }
                    to_client = "There were " + (stones + serv_step).ToString() + " stones. The server removed " + serv_step.ToString() + " .Remaining: " + stones.ToString() + ".";

                    await writer.WriteLineAsync(to_client);
                    await writer.FlushAsync();
                    if (int.TryParse(await reader.ReadLineAsync(), out client_step))
                    {
                        stones -= client_step;
                        if (stones < 1)
                        {
                            
                            await writer.WriteLineAsync("You are winner!");
                            await writer.FlushAsync();
                            game = false;
                            break;
                        }
                    }
                }
            }
            
        }
    }
}
