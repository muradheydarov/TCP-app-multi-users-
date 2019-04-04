using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientApp
{
    class Program
    {
        static string host = "127.0.0.1";
        static int port = 8051;
        static void Main(string[] args)
        {            
            TcpClient client = default(TcpClient);

            Console.Write("Enter your username: ");
            string username = Console.ReadLine();
            
            try
            {
                client = new TcpClient(host,port);
                NetworkStream stream = client.GetStream();
                string message = string.Empty;

                while (message != "close")
                {
                    Console.Write("Message: ");
                    message = Console.ReadLine();

                    string data = $"{username} : {message}";
                    byte[] dataArr = Encoding.UTF8.GetBytes(data);

                    stream.Write(dataArr, 0, dataArr.Length);

                    dataArr = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(dataArr, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(dataArr, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine($"From server {builder.ToString()}");                        
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
    }
}
