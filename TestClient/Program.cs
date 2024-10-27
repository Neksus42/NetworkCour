using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MySql.Data.MySqlClient;
namespace TestClient
{

    public class Customer
    {
        public int customer_id { get; set; }
        public string phone_number { get; set; }
        public string customer_name { get; set; }
    }





    internal class Program
    {

        public static async Task SendJsonOverTcpAsync(string ip, int port, byte[] jsonBytes)
        {
            using TcpClient client = new TcpClient();

            // Подключаемся к серверу
            await client.ConnectAsync(ip, port);
            Console.WriteLine("Соединение установлено.");

            using NetworkStream stream = client.GetStream();

            // Отправляем JSON-байты
            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
            Console.WriteLine("JSON отправлен.");

            // Чтение ответа сервера (опционально)
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Ответ сервера: " + response);
        }

        static async Task Main(string[] args)
        {
            string connectionString = "Server=localhost;Database=networkbase;User ID=root;Password=1111;";
            byte[] jsonBytes;


                try
                {
                MySqlConnection connection = new MySqlConnection(connectionString);
                    connection.Open();
                    Console.WriteLine("Соединение открыто!");
                string query = "SELECT * FROM networkbase.customers;";



                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Customer currentperson = new Customer();
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["customer_name"].ToString());
                            currentperson.customer_id = reader.GetInt32("customer_id");
                            currentperson.phone_number = reader.GetString("phone_number");
                            currentperson.customer_name = reader.GetString("customer_name");
                        }
                        jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize<Customer>(currentperson));
                        await SendJsonOverTcpAsync("localhost", 8888, jsonBytes);
                    }
                }
            }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }

            




        }


    }
}
