using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 8051;

            try
            {
                IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(iPAddress,port);

                listener.Start();
                Console.WriteLine("Waiting for connection...");

                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();
                    Console.WriteLine("Client Connected");

                    Client client = new Client(tcpClient);

                    Thread thread = new Thread(new ThreadStart(client.Process));
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    class Client
    {
        public TcpClient client;

        public Client(TcpClient tcpClient)
        {
            client = tcpClient;
        }

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                string message = string.Empty;

                while (message.ToLower() != "close")
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    } while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine(message);
                    message = message.Substring(message.IndexOf(':') + 1).Trim().ToUpper();

                    byte[] responseData = Encoding.UTF8.GetBytes(message);
                    stream.Write(responseData, 0, responseData.Length);
                }

                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
            }
        }
    }
}
