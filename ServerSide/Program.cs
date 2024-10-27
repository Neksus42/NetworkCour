
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace ServerSide
{

    public class Customer
    {
        public int customer_id { get; set; }
        public string phone_number { get; set; }
        public string customer_name { get; set; }
    }
    public class TcpJsonReceiver
    {
        public static async Task StartServerAsync(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Сервер запущен, ожидание подключения...");

            while (true)
            {
                using TcpClient client = await listener.AcceptTcpClientAsync();
                using NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string receivedJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Полученный JSON: " + receivedJson +"\n");
                Customer person = JsonSerializer.Deserialize<Customer>(receivedJson);
                // Опциональный ответ клиенту

                Console.WriteLine("Имя: " + person.customer_name);
                byte[] responseBytes = Encoding.UTF8.GetBytes("JSON получен");
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
        }
    }




    internal class Program
    {

        public static async Task Main(string[] args)
        {
            await TcpJsonReceiver.StartServerAsync(8888);
        }


    }
}

